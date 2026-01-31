using DG.Tweening;
using UnityEngine;

public class Starting : GameState
{
    private const float TransitionDuration = 4f;
    private const float CountdownSoundDelay = 4f;
    private const float TotalDuration = 6f;

    private Sequence _sequence;

    public override void Enter()
    {
        Debug.Log("Starting: Enter");

        // Fade out main menu music
        AudioManager.Instance?.FadeOutMusic();

        // Tween camera to InGame position
        var cameraAnchor = GameManager.Instance.InGameCameraAnchor;
        Camera.main.transform.DOMove(cameraAnchor.position, TransitionDuration).SetEase(Ease.InOutQuad);
        Camera.main.transform.DORotateQuaternion(cameraAnchor.rotation, TransitionDuration).SetEase(Ease.InOutQuad);

        // Tween players to InGame positions
        foreach (var player in GameManager.Instance.ActivePlayers)
        {
            var spawnPoint = GameManager.Instance.PlayerSpawnPointsInGame[player.PlayerId];
            player.transform.DOMove(spawnPoint.position, TransitionDuration).SetEase(Ease.InOutQuad);
            player.transform.DORotateQuaternion(spawnPoint.rotation, TransitionDuration).SetEase(Ease.InOutQuad);
        }

        // Create a sequence for timed events
        _sequence = DOTween.Sequence();

        // After 2 seconds, play countdown sound
        _sequence.InsertCallback(CountdownSoundDelay, () =>
        {
            AudioManager.Instance?.PlayCountdownSound();
        });

        // After 5 seconds, transition to InGame
        _sequence.InsertCallback(TotalDuration, () =>
        {
            GameManager.Instance.CurrentState = new InGame();
        });
    }

    public override void Exit()
    {
        Debug.Log("Starting: Exit");

        // Kill the sequence if still running
        _sequence?.Kill();
        _sequence = null;
    }
}