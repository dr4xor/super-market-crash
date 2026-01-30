using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReceiver : MonoBehaviour
{
    private PlayerInput _playerInput;
    
    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    private void OnInteract(InputValue inputValue)
    {
        
    }
}
