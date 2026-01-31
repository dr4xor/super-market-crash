using DG.Tweening;
using UnityEngine;

public class MainMenu : GameState
{
    private const float TransitionDuration = 1f;

    public override void Enter()
    {
        Debug.Log("MainMenu: Enter");

        AudioManager.Instance?.PlayMusic(AudioManager.MusicType.MainMenu);

        var cameraAnchor = GameManager.Instance.MainMenuCameraAnchor;
        Camera.main.transform.DOMove(cameraAnchor.position, TransitionDuration).SetEase(Ease.InOutQuad);
        Camera.main.transform.DORotateQuaternion(cameraAnchor.rotation, TransitionDuration).SetEase(Ease.InOutQuad);

        foreach (var player in GameManager.Instance.ActivePlayers)
        {
            var spawnPoint = GameManager.Instance.PlayerSpawnPointsMainMenu[player.PlayerId];
            player.transform.DOMove(spawnPoint.position, TransitionDuration).SetEase(Ease.InOutQuad);
            player.transform.DORotateQuaternion(spawnPoint.rotation, TransitionDuration).SetEase(Ease.InOutQuad);
        }
    }

    public override void Exit()
    {
        Debug.Log("MainMenu: Exit");
    }
}