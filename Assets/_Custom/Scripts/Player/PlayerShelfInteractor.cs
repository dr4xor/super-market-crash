using System;
using DG.Tweening;
using Scripts.Supermarket;
using UnityEngine;

public class PlayerShelfInteractor : MonoBehaviour
{
    [SerializeField] private CartItemsContainer itemsContainer;
    private ShelfFacade _shelfInRange = null;

    private Player _player;
    private UI_PickupHUD _pickupHud;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _shelfInRange = other.GetComponent<ShelfFacade>();
    }
    
    private void OnTriggerExit(Collider other)
    {
        _shelfInRange = null;
    }

    public void OnInteract()
    {
        if (_shelfInRange && _shelfInRange.HasItems)
        {
            _pickupHud = UI_Manager.Instance.SpawnPickupHUD(_shelfInRange.transform, _shelfInRange.itemTemplate, _player);
            var value = 0f;
            
            DOTween.To(() => 0f, x => _pickupHud.SetProgress(x), 1f, 1f)
                .SetEase(Ease.Linear)
                .SetLink(_pickupHud.gameObject);
            
            // DOTween.To(() => value, x => value = x, 1f, 1f)
            //     .OnUpdate(() =>
            //     {
            //         _pickupHud.SetProgress(value);
            //     })
            //     .OnComplete(() =>
            //     {
            //         if (_shelfInRange.TryTakeItem(out var item))
            //         {
            //             itemsContainer.AddItemToCart(item);
            //         }
            //     });
        }
    }
    
    public void OnInteractCancel()
    {
        Destroy(_pickupHud.gameObject);
    }
}
