using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Scripts.Supermarket;
using UnityEngine;

public class PlayerShelfInteractor : MonoBehaviour
{
    [SerializeField] private CartItemsContainer itemsContainer;

    private readonly HashSet<ShelfFacade> _shelvesInRange = new ();
    private ShelfFacade _closestShelf;
    private Player _player;
    private UI_PickupHUD _pickupHud;
    private TweenerCore<float, float, FloatOptions> _tween;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        UpdateClosestShelf();
    }

    private void UpdateClosestShelf()
    {
        ShelfFacade newClosest = null;
        var closestDistance = float.MaxValue;

        foreach (var shelf in _shelvesInRange)
        {
            if (!shelf)
            {
                continue;
            }

            var distance = Vector3.Distance(transform.position, shelf.transform.position);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ShelfFacade colliderShelf))
        {
            _shelvesInRange.Add(colliderShelf);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ShelfFacade colliderShelf))
        {
            _shelvesInRange.Remove(colliderShelf);
        }
    }

    public void OnInteract()
    {
        if (_closestShelf && _closestShelf.HasItems)
        {
            _tween = DOTween.To(() => 0f, x => _pickupHud.SetProgress(x), 1f, 2f)
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
                    }
                });
        }
    }
    
    public void OnInteractCancel()
    {
        if (_pickupHud)
        {
            _tween.Kill();
            _pickupHud.SetProgress(0f);
        }
    }
}
