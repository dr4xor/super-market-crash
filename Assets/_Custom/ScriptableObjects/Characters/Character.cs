using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
    public class Character : ScriptableObject
    {
        [Header("Character Info")]
        [Tooltip("Display name of the character")]
        public string characterName;
        
        [Tooltip("Icon shown in player HUD")]
        public Sprite icon;
        
        [Tooltip("The character model prefab to instantiate")]
        public GameObject characterPrefab;
        
        [Header("Skins/Variations")]
        [Tooltip("Available material variations (skins) for this character")]
        public Material[] skins;
        
        /// <summary>
        /// Gets the default skin material, or null if no skins available.
        /// </summary>
        public Material GetDefaultSkin()
        {
            if (skins == null || skins.Length == 0)
                return null;
            
            return skins[0];
        }
        
        /// <summary>
        /// Gets a skin by index, clamped to valid range.
        /// </summary>
        public Material GetSkin(int skinIndex)
        {
            if (skins == null || skins.Length == 0)
                return null;
            
            int index = Mathf.Clamp(skinIndex, 0, skins.Length - 1);
            return skins[index];
        }
        
        /// <summary>
        /// Returns the number of available skins for this character.
        /// </summary>
        public int GetSkinCount()
        {
            return skins?.Length ?? 0;
        }
    }
}
