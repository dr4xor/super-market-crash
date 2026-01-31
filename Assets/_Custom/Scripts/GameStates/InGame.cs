using DG.Tweening;
using UnityEngine;

public class InGame : GameState
{
    private const float TransitionDuration = 0f;

    public override void Enter()
    {
        Debug.Log("InGame: Enter");

        AudioManager.Instance?.PlayMusic(AudioManager.MusicType.Gameplay);

        var cameraAnchor = GameManager.Instance.InGameCameraAnchor;
        Camera.main.transform.DOMove(cameraAnchor.position, TransitionDuration).SetEase(Ease.InOutQuad);
        Camera.main.transform.DORotateQuaternion(cameraAnchor.rotation, TransitionDuration).SetEase(Ease.InOutQuad);

        foreach (var player in GameManager.Instance.ActivePlayers)
        {
            var spawnPoint = GameManager.Instance.PlayerSpawnPointsInGame[player.PlayerId];
            player.transform.DOMove(spawnPoint.position, TransitionDuration).SetEase(Ease.InOutQuad);
            player.transform.DORotateQuaternion(spawnPoint.rotation, TransitionDuration).SetEase(Ease.InOutQuad);
        }
        
        
        var shelfManager = ShelfsManager.Instance;
        if (shelfManager == null)
        {
            Debug.LogError("InGame: ShelfsManager not found");
            return;
        }

        var availableItems = shelfManager.GetAllItemTemplates();

        // Generate Shopping List for each player
        foreach (var player in GameManager.Instance.ActivePlayers)
        {
            var shoppingList = ShoppingListGenerator.Generate(availableItems);
            player.SetShoppingList(shoppingList);
        }

        UI_Manager.Instance.Show(UI_Manager.UIType.PlayerStats);
        UI_Manager.Instance.RefreshAllPlayers();
        UI_Manager.Instance.Hide(UI_Manager.UIType.MainMenu);
    }

    public override void Exit()
    {
        foreach (var player in GameManager.Instance.ActivePlayers)
        {
            player.SetShoppingList(null);
            player.ItemsInCart.Clear();
            player.BoughtItems.Clear();
        }
        Debug.Log("InGame: Exit");

        UI_Manager.Instance.Hide(UI_Manager.UIType.PlayerStats);
    }
}