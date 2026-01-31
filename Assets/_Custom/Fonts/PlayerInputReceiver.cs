using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReceiver : MonoBehaviour
{
    private PlayerInput _playerInput;
    private Cart _cart;
    private CartItemsContainer _cartItemsContainer;
    
    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _cart = GetComponent<Cart>();
        _cartItemsContainer = GetComponentInChildren<CartItemsContainer>();
    }

    private void OnInteract(InputValue inputValue)
    {
        
    }

    private void OnMove(InputValue inputValue)
    {
        Vector2 moveDirection = inputValue.Get<Vector2>();
        
        _cart.ProvideMoveDirection(moveDirection);
    }

    private void OnSouth(InputValue inputValue)
    {
        _cart.PerformDash();
    }

    private void OnNorth(InputValue inputValue)
    {
        _cartItemsContainer.InstantiateAndAddItemToCart();
    }

    private void OnEast(InputValue inputValue)
    {
    }

    private void OnWest(InputValue inputValue)
    {
        Debug.Log("OnWest");
        _cart.PlayerAnimationController.GrabItem();
    }
}
