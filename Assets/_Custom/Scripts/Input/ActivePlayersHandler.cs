using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class ActivePlayersHandler : MonoBehaviour
{
    [SerializeField] private PlayerInputManager playerInputManager;

    private List<PlayerInput> _joinedPlayers = new List<PlayerInput>();

    [ContextMenu("Start Listening For Input")]
    public void StartListeningForInput()
    {
        // Subscribe to any button press event
        InputSystem.onAnyButtonPress.Call(OnAnyButtonPress);
    }
    
    [ContextMenu("Stop Listening For Input")]
    public void StopListeningForInput()
    {
        // Unsubscribe from any button press event
        InputSystem.onAnyButtonPress.Call(ctrl => { });
    }

    private void OnAnyButtonPress(InputControl control)
    {
        // Get the device that triggered the input
        InputDevice device = control.device;
        
        // Check if this device is already being used by a player
        if (IsDeviceAlreadyUsed(device))
        {
            return;
        }

        // Join a new player with this device
        PlayerInput playerInput = playerInputManager.JoinPlayer(pairWithDevice: device);
        
        if (playerInput != null)
        {
            Debug.Log($"Player joined with device: {device.displayName}");

            _joinedPlayers.Add(playerInput);
            
            StopListeningForInput();
        }
    }

    private bool IsDeviceAlreadyUsed(InputDevice device)
    {
        // Check if any existing player is already using this device
        foreach (var playerInput in PlayerInput.all)
        {
            if (playerInput.devices.Contains(device))
            {
                return true;
            }
        }
        return false;
    }
}
