using System;
using Scripts.Supermarket;
using UnityEngine;

public class PlayerShelfInteractor : MonoBehaviour
{
    [SerializeField] private CartItemsContainer itemsContainer;
    private ShelfFacade _shelfInRange = null;
    
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
            if (_shelfInRange.TryTakeItem(out var item))
            {
                //itemsContainer.AddItem(item);
            }
        }
    }
}
