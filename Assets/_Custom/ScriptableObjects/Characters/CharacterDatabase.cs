using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Scriptable Objects/Character Database")]
    public class CharacterDatabase : ScriptableObject
    {
        [Header("Available Characters")]
        [Tooltip("List of all available characters in the game")]
        public List<Character> characters = new List<Character>();
        
        /// <summary>
        /// Gets a character by index. Returns null if index is out of range.
        /// </summary>
        public Character GetCharacter(int index)
        {
            if (characters == null || index < 0 || index >= characters.Count)
                return null;
            
            return characters[index];
        }
        
        /// <summary>
        /// Returns the total number of characters in the database.
        /// </summary>
        public int GetCharacterCount()
        {
            return characters?.Count ?? 0;
        }
        
        /// <summary>
        /// Validates that the database has at least one character and all entries are valid.
        /// </summary>
        public bool IsValid()
        {
            if (characters == null || characters.Count == 0)
                return false;
            
            foreach (var character in characters)
            {
                if (character == null)
                    return false;
                
                if (character.characterPrefab == null)
                {
                    Debug.LogWarning($"CharacterDatabase: Character '{character.characterName}' has no prefab assigned.");
                    return false;
                }
            }
            
            return true;
        }
    }
}
