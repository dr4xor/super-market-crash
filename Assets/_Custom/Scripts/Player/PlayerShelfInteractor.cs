using System;
using DG.Tweening;
using Scripts.Supermarket;
using UnityEngine;

public class PlayerShelfInteractor : MonoBehaviour
{
    [SerializeField] private CartItemsContainer itemsContainer;
    private ShelfFacade _shelfInRange;

    private Player _player;
    private UI_PickupHUD _pickupHud;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.name);
        _shelfInRange = other.GetComponent<ShelfFacade>();
        if (_shelfInRange)
        {
            _pickupHud = UI_Manager.Instance.SpawnPickupHUD(_shelfInRange.transform, _shelfInRange.itemTemplate, _player);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (_pickupHud)
        {
            Destroy(_pickupHud.gameObject);
        }
        _shelfInRange = null;
    }

    public void OnInteract()
    {
        print("Interact");
        if (_shelfInRange && _shelfInRange.HasItems)
        {
            DOTween.To(() => 0f, x => _pickupHud.SetProgress(x), 1f, 5f)
                .SetEase(Ease.Linear)
                .SetLink(_pickupHud.gameObject);
        }
    }
    
    public void OnInteractCancel()
    {
        print("InteractCancel");
        if (_pickupHud)
        {
            _pickupHud.SetProgress(0f);
        }
    }
}
