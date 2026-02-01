using UnityEngine;
using ScriptableObjects;

/// <summary>
/// Manages character display for render texture UI.
/// Instantiates player characters in designated positions for each of the 4 player slots.
/// Reacts to changes in active players and their character selections.
/// </summary>
[DefaultExecutionOrder(100)] // Run late to catch triggers set by other scripts
public class RenderTextureRoom : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera[] playerCameras = new Camera[4];

    [Header("Character Spawn Points")]
    [Tooltip("Parent transforms where character models will be instantiated (one per player slot)")]
    [SerializeField] private Transform[] characterParents = new Transform[4];

    [Header("Settings")]
    [Tooltip("Layer to assign to spawned characters for camera culling")]
    [SerializeField] private LayerMask characterLayer;

    // Tracks the currently spawned character instances per slot
    private GameObject[] _spawnedCharacters = new GameObject[4];

    // Tracks the last known character selection per player to detect changes
    private Character[] _lastKnownCharacters = new Character[4];
    private int[] _lastKnownSkinIndices = new int[4];

    // Cached renderer references for skin application
    private Renderer[][] _characterRenderers = new Renderer[4][];

    // Cached animators for the spawned render texture characters
    private Animator[] _spawnedAnimators = new Animator[4];

    // Cached references to source animators from player characters
    private Animator[] _sourceAnimators = new Animator[4];

    // Tracks trigger parameters for syncing (hashes and previous states)
    private int[][] _triggerHashes = new int[4][];
    private bool[][] _previousTriggerStates = new bool[4][];

    private GameManager _gameManager;

    private void Awake()
    {
        // Initialize skin indices to -1 to force initial spawn
        for (int i = 0; i < 4; i++)
        {
            _lastKnownSkinIndices[i] = -1;
        }
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;

        if (_gameManager == null)
        {
            Debug.LogError("RenderTextureRoom: GameManager not found!");
            return;
        }

        // Initial sync
        SyncAllCharacters();
    }

    private void Update()
    {
        if (_gameManager == null) return;

        // Check for changes in active players and their selections
        CheckForChanges();

        // Sync triggers - check before animator update consumes them
        SyncTriggers();
    }

    private void LateUpdate()
    {
        // Also sync in LateUpdate to catch triggers set late in the frame
        SyncTriggers();
    }

    /// <summary>
    /// Checks for any changes in player character selections and updates accordingly.
    /// </summary>
    private void CheckForChanges()
    {
        var activePlayers = _gameManager.ActivePlayers;

        // Track which slots are currently active
        bool[] activeSlots = new bool[4];

        foreach (var player in activePlayers)
        {
            int playerId = player.PlayerId;
            if (playerId < 0 || playerId >= 4) continue;

            activeSlots[playerId] = true;

            Character currentCharacter = player.SelectedCharacter;
            int currentSkinIndex = player.SelectedSkinIndex;

            // Check if character or skin has changed
            bool characterChanged = currentCharacter != _lastKnownCharacters[playerId];
            bool skinChanged = currentSkinIndex != _lastKnownSkinIndices[playerId];

            if (characterChanged)
            {
                SpawnCharacter(playerId, currentCharacter, currentSkinIndex);
            }
            else if (skinChanged && _spawnedCharacters[playerId] != null)
            {
                ApplySkin(playerId, currentCharacter, currentSkinIndex);
            }

            // Update source animator reference (in case player's character was re-instantiated)
            UpdateSourceAnimator(playerId, player);
        }

        // Clear characters for players that have left
        for (int i = 0; i < 4; i++)
        {
            if (!activeSlots[i] && _spawnedCharacters[i] != null)
            {
                ClearCharacter(i);
            }
        }
    }

    /// <summary>
    /// Forces a full sync of all characters based on current active players.
    /// </summary>
    public void SyncAllCharacters()
    {
        if (_gameManager == null) return;

        var activePlayers = _gameManager.ActivePlayers;
        bool[] activeSlots = new bool[4];

        foreach (var player in activePlayers)
        {
            int playerId = player.PlayerId;
            if (playerId < 0 || playerId >= 4) continue;

            activeSlots[playerId] = true;
            SpawnCharacter(playerId, player.SelectedCharacter, player.SelectedSkinIndex);
        }

        // Clear any slots that don't have active players
        for (int i = 0; i < 4; i++)
        {
            if (!activeSlots[i])
            {
                ClearCharacter(i);
            }
        }
    }

    /// <summary>
    /// Spawns a character model for the specified player slot.
    /// </summary>
    /// <param name="playerIndex">Player slot index (0-3)</param>
    /// <param name="character">Character ScriptableObject containing the prefab</param>
    /// <param name="skinIndex">Skin variation index</param>
    private void SpawnCharacter(int playerIndex, Character character, int skinIndex)
    {
        if (playerIndex < 0 || playerIndex >= 4) return;
        if (characterParents[playerIndex] == null)
        {
            Debug.LogWarning($"RenderTextureRoom: Character parent {playerIndex} is not assigned!");
            return;
        }

        // Clear existing character first
        ClearCharacter(playerIndex);

        if (character == null || character.characterPrefab == null)
        {
            Debug.LogWarning($"RenderTextureRoom: Character or prefab is null for player {playerIndex}");
            return;
        }

        Transform parent = characterParents[playerIndex];

        // Instantiate the character prefab
        GameObject characterInstance = Instantiate(
            character.characterPrefab,
            parent.position,
            parent.rotation,
            parent
        );

        // Reset local transform
        characterInstance.transform.localPosition = Vector3.zero;
        characterInstance.transform.localRotation = Quaternion.identity;

        // Store reference
        _spawnedCharacters[playerIndex] = characterInstance;

        // Cache renderers
        _characterRenderers[playerIndex] = characterInstance.GetComponentsInChildren<Renderer>(true);

        // Cache animator
        _spawnedAnimators[playerIndex] = characterInstance.GetComponentInChildren<Animator>(true);

        // Apply the character layer for camera culling (if specified)
        if (characterLayer != 0)
        {
            SetLayerRecursively(characterInstance, GetLayerFromMask(characterLayer));
        }

        // Apply skin
        ApplySkin(playerIndex, character, skinIndex);

        // Update tracking
        _lastKnownCharacters[playerIndex] = character;
        _lastKnownSkinIndices[playerIndex] = skinIndex;

        // Enable the corresponding camera
        if (playerCameras[playerIndex] != null)
        {
            playerCameras[playerIndex].enabled = true;
        }
    }

    /// <summary>
    /// Applies a skin material to the character at the specified slot.
    /// </summary>
    private void ApplySkin(int playerIndex, Character character, int skinIndex)
    {
        if (character == null) return;
        if (_characterRenderers[playerIndex] == null) return;

        Material skinMaterial = character.GetSkin(skinIndex);
        if (skinMaterial == null) return;

        foreach (var rend in _characterRenderers[playerIndex])
        {
            if (rend != null)
            {
                rend.material = skinMaterial;
            }
        }

        _lastKnownSkinIndices[playerIndex] = skinIndex;
    }

    /// <summary>
    /// Clears the character at the specified slot.
    /// </summary>
    /// <param name="playerIndex">Player slot index (0-3)</param>
    public void ClearCharacter(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= 4) return;

        if (_spawnedCharacters[playerIndex] != null)
        {
            Destroy(_spawnedCharacters[playerIndex]);
            _spawnedCharacters[playerIndex] = null;
        }

        _characterRenderers[playerIndex] = null;
        _spawnedAnimators[playerIndex] = null;
        _sourceAnimators[playerIndex] = null;
        _triggerHashes[playerIndex] = null;
        _previousTriggerStates[playerIndex] = null;
        _lastKnownCharacters[playerIndex] = null;
        _lastKnownSkinIndices[playerIndex] = -1;

        // Optionally disable the camera when no character is present
        if (playerCameras[playerIndex] != null)
        {
            playerCameras[playerIndex].enabled = false;
        }
    }

    /// <summary>
    /// Clears all spawned characters.
    /// </summary>
    public void ClearAllCharacters()
    {
        for (int i = 0; i < 4; i++)
        {
            ClearCharacter(i);
        }
    }

    /// <summary>
    /// Updates the source animator reference for a player slot.
    /// </summary>
    private void UpdateSourceAnimator(int playerIndex, Player player)
    {
        if (playerIndex < 0 || playerIndex >= 4) return;

        var playerCharacter = player.GetComponent<PlayerCharacter>();
        if (playerCharacter == null || playerCharacter.SpawnedCharacter == null)
        {
            _sourceAnimators[playerIndex] = null;
            _triggerHashes[playerIndex] = null;
            _previousTriggerStates[playerIndex] = null;
            return;
        }

        // Get the current animator from the player's spawned character
        Animator currentSourceAnimator = playerCharacter.SpawnedCharacter.GetComponentInChildren<Animator>(true);

        // Update if reference is null, destroyed, or changed to a different animator
        if (_sourceAnimators[playerIndex] == null || _sourceAnimators[playerIndex] != currentSourceAnimator)
        {
            _sourceAnimators[playerIndex] = currentSourceAnimator;

            // Cache parameters when we get a new source animator
            CacheAnimatorParameters(playerIndex, _sourceAnimators[playerIndex]);
        }
    }

    /// <summary>
    /// Syncs trigger parameters from source to target animators.
    /// Detects when a trigger is activated and replicates it to the target.
    /// </summary>
    private void SyncTriggers()
    {
        for (int i = 0; i < 4; i++)
        {
            Animator source = _sourceAnimators[i];
            Animator target = _spawnedAnimators[i];

            if (source == null || target == null) continue;
            if (_triggerHashes[i] == null || _previousTriggerStates[i] == null) continue;

            var hashes = _triggerHashes[i];
            var prevStates = _previousTriggerStates[i];

            for (int t = 0; t < hashes.Length; t++)
            {
                int hash = hashes[t];
                bool currentState = source.GetBool(hash);
                bool prevState = prevStates[t];

                // Detect rising edge: trigger was just activated (false -> true)
                if (currentState && !prevState)
                {
                    target.SetTrigger(hash);
                }

                // Update previous state
                prevStates[t] = currentState;
            }
        }
    }

    /// <summary>
    /// Caches animator parameters for a player slot when the source animator is set.
    /// Specifically extracts trigger parameters for efficient syncing.
    /// </summary>
    private void CacheAnimatorParameters(int playerIndex, Animator source)
    {
        if (source == null)
        {
            _triggerHashes[playerIndex] = null;
            _previousTriggerStates[playerIndex] = null;
            return;
        }

        // Count triggers and cache their hashes
        int triggerCount = 0;
        foreach (var param in source.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
                triggerCount++;
        }

        _triggerHashes[playerIndex] = new int[triggerCount];
        _previousTriggerStates[playerIndex] = new bool[triggerCount];

        int index = 0;
        foreach (var param in source.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                _triggerHashes[playerIndex][index] = param.nameHash;
                _previousTriggerStates[playerIndex][index] = source.GetBool(param.nameHash);
                index++;
            }
        }
    }

    /// <summary>
    /// Sets the layer recursively for a GameObject and all its children.
    /// </summary>
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        if (obj == null) return;

        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    /// <summary>
    /// Extracts the first layer index from a LayerMask.
    /// </summary>
    private int GetLayerFromMask(LayerMask mask)
    {
        int layerNumber = 0;
        int layer = mask.value;
        while (layer > 1)
        {
            layer >>= 1;
            layerNumber++;
        }
        return layerNumber;
    }

    /// <summary>
    /// Gets the camera for a specific player slot.
    /// </summary>
    public Camera GetPlayerCamera(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= 4) return null;
        return playerCameras[playerIndex];
    }

    /// <summary>
    /// Gets the spawned character instance for a specific player slot.
    /// </summary>
    public GameObject GetSpawnedCharacter(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= 4) return null;
        return _spawnedCharacters[playerIndex];
    }

    private void OnDestroy()
    {
        ClearAllCharacters();
    }
}