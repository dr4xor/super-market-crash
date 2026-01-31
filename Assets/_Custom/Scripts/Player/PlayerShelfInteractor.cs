using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Scripts.Supermarket;
using UnityEngine;

public class PlayerShelfInteractor : MonoBehaviour
{
    [SerializeField] private CartItemsContainer itemsContainer;
    private ShelfFacade _shelfInRange;

    private Player _player;
    private UI_PickupHUD _pickupHud;
    private TweenerCore<float, float, FloatOptions> _tween;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ShelfFacade colliderShelf))
        {
            _shelfInRange = colliderShelf;
            if (_pickupHud)
            {
                Destroy(_pickupHud.gameObject);
            }
            if (_shelfInRange.HasItems)
            {
                _pickupHud = UI_Manager.Instance.SpawnPickupHUD(_shelfInRange.transform, _shelfInRange.itemTemplate.sprite, _player);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ShelfFacade colliderShelf))
        {
            if (colliderShelf == _shelfInRange)
            {
                if (_pickupHud)
                {
                    Destroy(_pickupHud.gameObject);
                }

                _shelfInRange = null;
            }
        }
    }

    public void OnInteract()
    {
        if (_shelfInRange && _shelfInRange.HasItems)
        {
            _tween = DOTween.To(() => 0f, x => _pickupHud.SetProgress(x), 1f, 2f)
                .SetEase(Ease.Linear)
                .SetLink(_pickupHud.gameObject)
                .OnComplete(() =>
                {
                    if (_shelfInRange.TryTakeItem(out var item))
                    {
                        itemsContainer.AddItemToCart(item);
                        if (_shelfInRange.HasItems == false)
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
