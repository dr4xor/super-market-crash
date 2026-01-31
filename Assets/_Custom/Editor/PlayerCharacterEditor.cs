using ScriptableObjects;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerCharacter))]
public class PlayerCharacterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var playerCharacter = (PlayerCharacter)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Editor Controls", EditorStyles.boldLabel);

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Character cycling only available during Play mode.", MessageType.Info);
            return;
        }

        var gameManager = GameManager.Instance;
        if (gameManager == null || gameManager.CharacterDatabase == null)
        {
            EditorGUILayout.HelpBox("GameManager with CharacterDatabase not found.", MessageType.Warning);
            return;
        }

        var characterDatabase = gameManager.CharacterDatabase;
        if (characterDatabase.GetCharacterCount() == 0)
        {
            EditorGUILayout.HelpBox("CharacterDatabase is empty.", MessageType.Warning);
            return;
        }

        // Get current character index
        int currentCharacterIndex = 0;
        if (playerCharacter.CurrentCharacter != null)
        {
            int foundIndex = characterDatabase.characters.IndexOf(playerCharacter.CurrentCharacter);
            if (foundIndex >= 0)
                currentCharacterIndex = foundIndex;
        }

        EditorGUILayout.Space(5);

        // Character selection
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Character:", GUILayout.Width(70));

        if (GUILayout.Button("◄", GUILayout.Width(30)))
        {
            int prevIndex = currentCharacterIndex - 1;
            if (prevIndex < 0) prevIndex = characterDatabase.GetCharacterCount() - 1;
            playerCharacter.SetCharacter(characterDatabase.GetCharacter(prevIndex), 0);
        }

        string charName = playerCharacter.CurrentCharacter != null
            ? playerCharacter.CurrentCharacter.characterName
            : "None";
        string charLabel = $"{charName} ({currentCharacterIndex + 1}/{characterDatabase.GetCharacterCount()})";
        EditorGUILayout.LabelField(charLabel, EditorStyles.centeredGreyMiniLabel);

        if (GUILayout.Button("►", GUILayout.Width(30)))
        {
            int nextIndex = (currentCharacterIndex + 1) % characterDatabase.GetCharacterCount();
            playerCharacter.SetCharacter(characterDatabase.GetCharacter(nextIndex), 0);
        }

        EditorGUILayout.EndHorizontal();

        // Skin selection
        if (playerCharacter.CurrentCharacter != null && playerCharacter.CurrentCharacter.GetSkinCount() > 0)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Skin:", GUILayout.Width(70));

            if (GUILayout.Button("◄", GUILayout.Width(30)))
            {
                playerCharacter.PreviousSkin();
            }

            string skinLabel = $"Skin {playerCharacter.CurrentSkinIndex + 1}/{playerCharacter.CurrentCharacter.GetSkinCount()}";
            EditorGUILayout.LabelField(skinLabel, EditorStyles.centeredGreyMiniLabel);

            if (GUILayout.Button("►", GUILayout.Width(30)))
            {
                playerCharacter.NextSkin();
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(5);

        // Current state info
        EditorGUILayout.HelpBox(playerCharacter.GetCharacterInfo(), MessageType.None);
    }
}