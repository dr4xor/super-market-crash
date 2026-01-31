using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
[CanEditMultipleObjects]
public class GameManagerEditor : Editor
{
    private enum StateOption { MainMenu, InGame }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (targets.Length > 1)
            return;

        var gameManager = (GameManager)target;

        if (!Application.isPlaying)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("State switching only available during Play mode.", MessageType.Info);
            return;
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("State Control", EditorStyles.boldLabel);

        StateOption currentOption = gameManager.IsInMainMenu ? StateOption.MainMenu : StateOption.InGame;
        StateOption newOption = (StateOption)EditorGUILayout.EnumPopup("Current State", currentOption);

        if (newOption != currentOption)
        {
            switch (newOption)
            {
                case StateOption.MainMenu:
                    gameManager.CurrentState = new MainMenu();
                    break;
                case StateOption.InGame:
                    gameManager.CurrentState = new InGame();
                    break;
            }
        }
    }
}
