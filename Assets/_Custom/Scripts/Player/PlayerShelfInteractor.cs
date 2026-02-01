using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Scripts.Supermarket;
using UnityEngine;

public class PlayerShelfInteractor : MonoBehaviour
{
    [SerializeField] private CartItemsContainer itemsContainer;
    [SerializeField] private AudioClip clipCollectItem;
    [SerializeField] private Transform distanceCheckPosition;
    [SerializeField] private float timeToPickupItems = 2f;
    [SerializeField] private PlayerShelfInRangeChecker playerShelfInRangeChecker;

    private ShelfFacade _closestShelf;
    private Player _player;
    private UI_PickupHUD _pickupHud;
    private TweenerCore<float, float, FloatOptions> _tween;

    private AudioSource _audioSource;
    private PlayerAnimationController _playerAnimationController;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.clip = clipCollectItem;
        _audioSource.playOnAwake = false;
        _audioSource.volume = 1f;
        _audioSource.spatialBlend = 1f;
        _audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        _audioSource.dopplerLevel = 0f;
        _audioSource.minDistance = 5f;
        _audioSource.maxDistance = 500f;
    }

    private void Update()
    {
        UpdateClosestShelf();
    }

    private void UpdateClosestShelf()
    {
        ShelfFacade newClosest = null;
        var closestDistance = float.MaxValue;

        foreach (var shelf in playerShelfInRangeChecker.ShelvesInRange)
        {
            if (!shelf)
            {
                continue;
            }

            Debug.DrawLine(distanceCheckPosition.position, shelf.transform.position, Color.red);
            var distance = Vector3.Distance(distanceCheckPosition.position, shelf.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                newClosest = shelf;
            }
        }

        if (newClosest != _closestShelf)
        {
            _closestShelf = newClosest;
            UpdateHUD();
        }
        else if (_closestShelf && !_pickupHud && _closestShelf.HasItems)
        {
            UpdateHUD();
        }
    }

    private void UpdateHUD()
    {
        if (_pickupHud)
        {
            Destroy(_pickupHud.gameObject);
            _pickupHud = null;
        }

        if (_closestShelf && _closestShelf.HasItems)
        {
            _pickupHud = UI_Manager.Instance.SpawnPickupHUD(_closestShelf.hudPosition, _closestShelf.itemTemplate.sprite, _player);
        }
    }

    public void OnInteract()
    {
        if (_closestShelf && _closestShelf.HasItems)
        {
            _playerAnimationController.GrabItem();
            _audioSource.Play();
            _tween = DOTween.To(() => 0f, x => _pickupHud.SetProgress(x), 1f, timeToPickupItems)
                .SetEase(Ease.Linear)
                .SetLink(_pickupHud.gameObject)
                .OnComplete(() =>
                {
                    if (_closestShelf.TryTakeItem(out var item))
                    {
                        itemsContainer.AddItemToCart(item);
                        if (_closestShelf.HasItems == false)
                        {
                            Destroy(_pickupHud.gameObject);
                        }
                        _audioSource.Stop();
                    }
                });
        }
    }
    
    public void OnInteractCancel()
    {
        if (_pickupHud)
        {
            _audioSource.Stop();
            _tween.Kill();
            _pickupHud.SetProgress(0f);
        }
    }
}
