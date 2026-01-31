using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReceiver : MonoBehaviour
{
    private PlayerInput _playerInput;
    private Cart _cart;
    private CartItemsContainer _cartItemsContainer;
    private PlayerShelfInteractor _playerShelfInteractor;
    private PlayerCheckoutInteractor _playerCheckoutInteractor;
    private PlayerCharacter _playerCharacter;

    private bool _characterSwitchReady = true;

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _cart = GetComponent<Cart>();
        _cartItemsContainer = GetComponentInChildren<CartItemsContainer>();
        _playerShelfInteractor = GetComponent<PlayerShelfInteractor>();
        _playerCheckoutInteractor = GetComponent<PlayerCheckoutInteractor>();
        _playerCharacter = GetComponent<PlayerCharacter>();

        var eastAction = _playerInput.actions["East"];
        eastAction.started += OnEastDown;
        eastAction.canceled += OnEastUp;

    }

    private void OnInteract(InputValue inputValue)
    {
    }

    private void OnMove(InputValue inputValue)
    {
        Vector2 moveDirection = inputValue.Get<Vector2>();

        if (GameManager.Instance != null && GameManager.Instance.IsInMainMenu)
        {
            // Character switching with horizontal input
            if (_characterSwitchReady)
            {
                if (moveDirection.x > 0.5f)
                {
                    _playerCharacter.SwitchCharacter(1);
                    _characterSwitchReady = false;
                }
                else if (moveDirection.x < -0.5f)
                {
                    _playerCharacter.SwitchCharacter(-1);
                    _characterSwitchReady = false;
                }
            }

            // Reset when stick returns to center
            if (Mathf.Abs(moveDirection.x) < 0.3f)
                _characterSwitchReady = true;

            _cart.ProvideMoveDirection(Vector2.zero);
            return;
        }

        _cart.ProvideMoveDirection(moveDirection);
    }


    private void OnSouth(InputValue inputValue)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsInMainMenu)
            return;

        _cart.PerformDash();
    }

    private void OnNorth(InputValue inputValue)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsInMainMenu)
            return;

        _cartItemsContainer.InstantiateAndAddItemToCart();
    }

    private void OnEast(InputValue inputValue)
    {
    }

    private void OnWest(InputValue inputValue)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsInMainMenu)
        {
            _playerCharacter.SwitchVariation(1);
            return;
        }

        _cart.PlayerAnimationController.GrabItem();
    }

    private void OnEastUp(InputAction.CallbackContext context)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsInMainMenu)
            return;

        Debug.Log("OnEastUp");
        _playerShelfInteractor.OnInteractCancel();
        _playerCheckoutInteractor.OnInteractCancel();
    }

    private void OnEastDown(InputAction.CallbackContext context)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsInMainMenu)
            return;

        Debug.Log("OnEastDown");
        _playerShelfInteractor.OnInteract();
        _playerCheckoutInteractor.OnInteract();
    }
}