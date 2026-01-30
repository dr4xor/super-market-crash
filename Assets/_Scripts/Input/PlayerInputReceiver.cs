using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReceiver : MonoBehaviour
{
    private PlayerInput _playerInput;
    private Cart _cart;
    
    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _cart = GetComponent<Cart>();
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
        
    }

    private void OnEast(InputValue inputValue)
    {
        
    }

    private void OnWest(InputValue inputValue)
    {
        
    }
}
