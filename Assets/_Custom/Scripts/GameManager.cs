using ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public enum GameState { MainMenu, InGame }

    public static GameManager Instance { get; private set; }

    [Header("State")]
    [SerializeField] private GameState currentState = GameState.MainMenu;

    [Header("References")]
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private CharacterDatabase characterDatabase;

    [Header("Prefabs")]
    [SerializeField] private UI_Manager uiManagerPrefab;

    private UI_Manager _uiManagerInstance;
    private readonly bool[] _usedPlayerIds = new bool[4];

    // Track all active players
    private readonly List<Player> _activePlayers = new List<Player>();

    public GameState CurrentState
    {
        get => currentState;
        set => currentState = value;
    }

    public bool IsInMainMenu => currentState == GameState.MainMenu;
    public bool IsInGame => currentState == GameState.InGame;

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

        // Set the default character selection for the player
        player.SelectedCharacter = characterDatabase.GetCharacter(0);
        player.SelectedSkinIndex = 0;

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

        // Remove from active players
        _activePlayers.Remove(player);

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

        player.SelectedCharacter = character;
        player.SelectedSkinIndex = skinIndex;

        Debug.Log($"GameManager: Player {player.PlayerId} selected character '{character.characterName}' with skin {skinIndex}");
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

        foreach (var player in _activePlayers)
        {
            if (player != excludePlayer && player.SelectedCharacter == character)
                return true;
        }
        return false;
    }
}
