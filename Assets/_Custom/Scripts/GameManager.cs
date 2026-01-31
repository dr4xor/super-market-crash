using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ItemDatabase itemDatabase;

    [Header("Prefabs")]
    [SerializeField] private UI_Manager uiManagerPrefab;

    private UI_Manager _uiManagerInstance;
    private readonly bool[] _usedPlayerIds = new bool[4];

    private void Start()
    {
        EnsureUIManagerExists();
    }

    /// <summary>
    /// Called by PlayerInputManager via SendMessage when Notification Behavior is set to SendMessages.
    /// </summary>
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        if (playerInput == null) return;

        var player = playerInput.GetComponent<Player>();
        if (player == null)
        {
            Debug.LogWarning("GameManager: Spawned player has no Player component.");
            return;
        }

        EnsureUIManagerExists();
        if (_uiManagerInstance == null) return;

        int playerId = GetNextFreePlayerId();
        if (playerId < 0)
        {
            Debug.LogWarning("GameManager: Max players (4) reached. Ignoring join.");
            return;
        }

        player.PlayerId = playerId;
        _usedPlayerIds[playerId] = true;

        if (itemDatabase != null)
        {
            var shoppingList = ShoppingListGenerator.Generate(itemDatabase);
            player.SetShoppingList(shoppingList);
        }

        var stats = _uiManagerInstance.AddPlayer(player);
        if (stats != null)
            stats.SetPlayerName($"Player {playerId + 1}");
    }

    /// <summary>
    /// Called by PlayerInputManager via SendMessage when Notification Behavior is set to SendMessages.
    /// </summary>
    private void OnPlayerLeft(PlayerInput playerInput)
    {
        if (playerInput == null) return;

        var player = playerInput.GetComponent<Player>();
        if (player == null) return;

        _usedPlayerIds[player.PlayerId] = false;

        if (_uiManagerInstance != null)
            _uiManagerInstance.RemovePlayer(player);
    }

    private void EnsureUIManagerExists()
    {
        if (_uiManagerInstance != null) return;

        if (uiManagerPrefab == null)
        {
            Debug.LogWarning("GameManager: UI_Manager prefab not set.");
            return;
        }

        _uiManagerInstance = FindObjectOfType<UI_Manager>();
        if (_uiManagerInstance == null)
            _uiManagerInstance = Instantiate(uiManagerPrefab);
    }

    private int GetNextFreePlayerId()
    {
        for (int i = 0; i < _usedPlayerIds.Length; i++)
        {
            if (!_usedPlayerIds[i])
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns the runtime UI_Manager instance (after Start has run), or null.
    /// </summary>
    public UI_Manager UIManager => _uiManagerInstance;
}
