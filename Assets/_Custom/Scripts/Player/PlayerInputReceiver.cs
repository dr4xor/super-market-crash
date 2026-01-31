using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReceiver : MonoBehaviour
{
    private PlayerInput _playerInput;
    private Cart _cart;
    private CartItemsContainer _cartItemsContainer;
    private PlayerShelfInteractor _playerShelfInteractor;
    private PlayerCharacter _playerCharacter;

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _cart = GetComponent<Cart>();
        _cartItemsContainer = GetComponentInChildren<CartItemsContainer>();
        _playerShelfInteractor = GetComponent<PlayerShelfInteractor>();
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
            // Cant move while in main menu
            _cart.ProvideMoveDirection(Vector2.zero);
            return;
        }

        _cart.ProvideMoveDirection(moveDirection);
    }

    private void OnNavigate(InputValue inputValue)
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsInMainMenu)
            return;

        Vector2 input = inputValue.Get<Vector2>();

        if (input.x > 0.5f)
            _playerCharacter.SwitchCharacter(1);
        else if (input.x < -0.5f)
            _playerCharacter.SwitchCharacter(-1);
    }

    private void OnRightShoulder(InputValue inputValue)
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsInMainMenu)
            return;

        _playerCharacter.SwitchVariation(1);
    }

    private void OnLeftShoulder(InputValue inputValue)
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsInMainMenu)
            return;

        _playerCharacter.SwitchVariation(-1);
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
            return;

        Debug.Log("OnWest");
        _cart.PlayerAnimationController.GrabItem();
    }

    private void OnEastUp(InputAction.CallbackContext context)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsInMainMenu)
            return;

        Debug.Log("OnEastUp");
        _playerShelfInteractor.OnInteractCancel();
    }

    private void OnEastDown(InputAction.CallbackContext context)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsInMainMenu)
            return;

        Debug.Log("OnEastDown");
        _playerShelfInteractor.OnInteract();
    }
}