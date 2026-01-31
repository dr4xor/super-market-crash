using ScriptableObjects;
using UnityEngine;

/// <summary>
/// Handles character model instantiation and skin switching for a player.
/// Attach this to the Player GameObject.
/// </summary>
public class PlayerCharacter : MonoBehaviour
{
    [Header("Character Settings")]
    [SerializeField] private Transform characterAttachPoint;
    
    [Header("Current Selection")]
    [SerializeField] private Character currentCharacter;
    [SerializeField] private int currentSkinIndex = 0;
    
    private GameObject _spawnedCharacterInstance;
    private Renderer[] _characterRenderers;
    
    public Character CurrentCharacter => currentCharacter;
    public int CurrentSkinIndex => currentSkinIndex;
    public GameObject SpawnedCharacter => _spawnedCharacterInstance;
    
    private void Awake()
    {
        // If no attach point is specified, use this transform as the attach point
        if (characterAttachPoint == null)
            characterAttachPoint = transform;
    }
    
    /// <summary>
    /// Sets the character for this player. Destroys previous character if exists.
    /// </summary>
    /// <param name="character">The character ScriptableObject to instantiate</param>
    /// <param name="skinIndex">The skin index to apply (optional, defaults to 0)</param>
    public void SetCharacter(Character character, int skinIndex = 0)
    {
        if (character == null)
        {
            Debug.LogWarning("PlayerCharacter: Cannot set null character.");
            return;
        }
        
        if (character.characterPrefab == null)
        {
            Debug.LogWarning($"PlayerCharacter: Character '{character.characterName}' has no prefab assigned.");
            return;
        }
        
        // Destroy previous character if exists
        ClearCharacter();
        
        // Store current selection
        currentCharacter = character;
        currentSkinIndex = Mathf.Clamp(skinIndex, 0, character.GetSkinCount() - 1);
        
        // Instantiate new character
        _spawnedCharacterInstance = Instantiate(
            character.characterPrefab, 
            characterAttachPoint.position,
            characterAttachPoint.rotation,
            characterAttachPoint
        );
        
        // Reset local transform to ensure proper positioning
        _spawnedCharacterInstance.transform.localPosition = Vector3.zero;
        _spawnedCharacterInstance.transform.localRotation = Quaternion.identity;
        
        // Cache renderers for skin switching
        _characterRenderers = _spawnedCharacterInstance.GetComponentsInChildren<Renderer>(true);
        
        // Apply the selected skin
        ApplySkin(currentSkinIndex);
        
        Debug.Log($"PlayerCharacter: Set character to '{character.characterName}' with skin index {currentSkinIndex}");
    }
    
    /// <summary>
    /// Changes the skin/material of the current character.
    /// </summary>
    /// <param name="skinIndex">The skin index to apply</param>
    public void SetSkin(int skinIndex)
    {
        if (currentCharacter == null)
        {
            Debug.LogWarning("PlayerCharacter: No character set. Cannot change skin.");
            return;
        }
        
        currentSkinIndex = Mathf.Clamp(skinIndex, 0, currentCharacter.GetSkinCount() - 1);
        ApplySkin(currentSkinIndex);
    }
    
    /// <summary>
    /// Cycles to the next skin variation.
    /// </summary>
    public void NextSkin()
    {
        if (currentCharacter == null || currentCharacter.GetSkinCount() == 0)
            return;
        
        int nextIndex = (currentSkinIndex + 1) % currentCharacter.GetSkinCount();
        SetSkin(nextIndex);
    }
    
    /// <summary>
    /// Cycles to the previous skin variation.
    /// </summary>
    public void PreviousSkin()
    {
        if (currentCharacter == null || currentCharacter.GetSkinCount() == 0)
            return;
        
        int prevIndex = currentSkinIndex - 1;
        if (prevIndex < 0)
            prevIndex = currentCharacter.GetSkinCount() - 1;
        
        SetSkin(prevIndex);
    }
    
    /// <summary>
    /// Removes the current character model.
    /// </summary>
    public void ClearCharacter()
    {
        if (_spawnedCharacterInstance != null)
        {
            Destroy(_spawnedCharacterInstance);
            _spawnedCharacterInstance = null;
            _characterRenderers = null;
        }
        
        currentCharacter = null;
        currentSkinIndex = 0;

        // Check that the character attach transform has no children anymore
        if (characterAttachPoint.childCount > 0)
        {
            Debug.LogWarning("PlayerCharacter: Character attach transform has children. Destroying them.");
            foreach (Transform child in characterAttachPoint)
            {
                Destroy(child.gameObject);
            }
        }
    }
    
    /// <summary>
    /// Applies the material to all renderers on the character.
    /// </summary>
    private void ApplySkin(int skinIndex)
    {
        if (currentCharacter == null || _characterRenderers == null)
            return;
        
        Material skinMaterial = currentCharacter.GetSkin(skinIndex);
        if (skinMaterial == null)
        {
            Debug.LogWarning($"PlayerCharacter: No skin material at index {skinIndex} for character '{currentCharacter.characterName}'");
            return;
        }
        
        // Apply material to all renderers
        foreach (var renderer in _characterRenderers)
        {
            if (renderer != null)
            {
                // Create a material instance to avoid modifying the shared material
                renderer.material = skinMaterial;
            }
        }
        
        Debug.Log($"PlayerCharacter: Applied skin index {skinIndex} to character '{currentCharacter.characterName}'");
    }
    
    /// <summary>
    /// Gets character and skin info as a formatted string.
    /// </summary>
    public string GetCharacterInfo()
    {
        if (currentCharacter == null)
            return "No character selected";

        return $"{currentCharacter.characterName} (Skin {currentSkinIndex + 1}/{currentCharacter.GetSkinCount()})";
    }

    /// <summary>
    /// Switches to the next or previous character in the database.
    /// </summary>
    /// <param name="direction">1 for next, -1 for previous</param>
    public void SwitchCharacter(int direction)
    {
        var db = GameManager.Instance?.CharacterDatabase;
        if (db == null || db.GetCharacterCount() == 0)
            return;

        var player = GetComponent<Player>();
        int currentIndex = db.characters.IndexOf(player.SelectedCharacter);
        if (currentIndex < 0) currentIndex = 0;

        int newIndex = (currentIndex + direction + db.GetCharacterCount()) % db.GetCharacterCount();
        var newCharacter = db.GetCharacter(newIndex);

        player.SelectedCharacter = newCharacter;
        player.SelectedSkinIndex = 0;
        SetCharacter(newCharacter, 0);
    }

    /// <summary>
    /// Switches to the next or previous skin variation.
    /// </summary>
    /// <param name="direction">1 for next, -1 for previous</param>
    public void SwitchVariation(int direction)
    {
        var player = GetComponent<Player>();
        if (player.SelectedCharacter == null)
            return;

        int skinCount = player.SelectedCharacter.GetSkinCount();
        if (skinCount == 0) return;

        int newSkin = (player.SelectedSkinIndex + direction + skinCount) % skinCount;
        player.SelectedSkinIndex = newSkin;
        SetSkin(newSkin);
    }
}
