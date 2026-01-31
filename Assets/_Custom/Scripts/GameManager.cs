using ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private CharacterDatabase characterDatabase;

    [Header("Prefabs")]
    [SerializeField] private UI_Manager uiManagerPrefab;

    private UI_Manager _uiManagerInstance;
    private readonly bool[] _usedPlayerIds = new bool[4];

    // Track all active players
    private readonly List<Player> _activePlayers = new List<Player>();

    // Track character selections: Player -> CharacterSelection
    private readonly Dictionary<Player, CharacterSelection> _characterSelections = new Dictionary<Player, CharacterSelection>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

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
        
        // Add to active players list
        _activePlayers.Add(player);

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

        int playerId = player.PlayerId;
        _usedPlayerIds[playerId] = false;

        // Remove from active players and clear character selection
        _activePlayers.Remove(player);
        _characterSelections.Remove(player);

        if (_uiManagerInstance != null)
            _uiManagerInstance.RemovePlayer(player);

        // Notify MainMenuManager
        // var mainMenuManager = GetComponent<MainMenuManager>();
        // if (mainMenuManager != null)
        //     mainMenuManager.OnPlayerLeft(player);
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
    
    /// <summary>
    /// Returns the character database reference.
    /// </summary>
    public CharacterDatabase CharacterDatabase => characterDatabase;
    
    /// <summary>
    /// Returns a read-only list of all active players.
    /// </summary>
    public IReadOnlyList<Player> ActivePlayers => _activePlayers;
    
    /// <summary>
    /// Sets the character and skin selection for a specific player.
    /// </summary>
    /// <param name="player">The player making the selection</param>
    /// <param name="character">The selected character ScriptableObject</param>
    /// <param name="skinIndex">The index of the selected skin variation</param>
    public void SetCharacterSelection(Player player, Character character, int skinIndex)
    {
        if (player == null)
        {
            Debug.LogWarning("GameManager: Cannot set character selection for null player.");
            return;
        }
        
        if (character == null)
        {
            Debug.LogWarning("GameManager: Cannot set null character.");
            return;
        }
        
        if (skinIndex < 0 || skinIndex >= character.GetSkinCount())
        {
            Debug.LogWarning($"GameManager: Invalid skinIndex {skinIndex}. Character '{character.characterName}' has {character.GetSkinCount()} skins.");
            skinIndex = Mathf.Clamp(skinIndex, 0, character.GetSkinCount() - 1);
        }
        
        _characterSelections[player] = new CharacterSelection
        {
            character = character,
            skinIndex = skinIndex
        };
        
        Debug.Log($"GameManager: Player {player.PlayerId} selected character '{character.characterName}' with skin {skinIndex}");
    }
    
    /// <summary>
    /// Gets the character selection for a specific player. Returns null if not set.
    /// </summary>
    public CharacterSelection? GetCharacterSelection(Player player)
    {
        if (player == null)
            return null;
            
        if (_characterSelections.TryGetValue(player, out var selection))
            return selection;
        
        return null;
    }
    
    /// <summary>
    /// Checks if a character is already selected by any player (useful for preventing duplicates).
    /// </summary>
    /// <param name="character">The character to check</param>
    /// <param name="excludePlayer">Optional player to exclude from the check (e.g., the current player)</param>
    /// <returns>True if another player has selected this character</returns>
    public bool IsCharacterSelected(Character character, Player excludePlayer = null)
    {
        if (character == null)
            return false;
            
        foreach (var kvp in _characterSelections)
        {
            if (kvp.Key != excludePlayer && kvp.Value.character == character)
                return true;
        }
        return false;
    }
    
    /// <summary>
    /// Gets all current character selections.
    /// </summary>
    public Dictionary<Player, CharacterSelection> GetAllCharacterSelections()
    {
        return new Dictionary<Player, CharacterSelection>(_characterSelections);
    }
}

/// <summary>
/// Data structure to store a player's character selection.
/// </summary>
[System.Serializable]
public struct CharacterSelection
{
    public Character character;
    public int skinIndex;
}
