using System.Collections.Generic;
using UnityEngine;
using ScriptableObjects;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform playerStatsContainer;
    [SerializeField] private UI_PlayerStats playerStatsPrefab;
    [SerializeField] private UI_PickupHUD pickupHudPrefab;
    [SerializeField] private Transform pickupHudContainer;

    private readonly Dictionary<Player, UI_PlayerStats> _playerStatsByPlayer = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>
    /// Spawns a UI_PlayerStats for the given player and binds it. Call when a player connects.
    /// </summary>
    /// <param name="player">The connected player.</param>
    /// <returns>The created UI_PlayerStats instance, or null if prefab/container not set.</returns>
    public UI_PlayerStats AddPlayer(Player player)
    {
        if (player == null || playerStatsPrefab == null || playerStatsContainer == null)
            return null;

        if (_playerStatsByPlayer.TryGetValue(player, out var existing))
            return existing;

        var instance = Instantiate(playerStatsPrefab, playerStatsContainer);
        instance.Bind(player);
        _playerStatsByPlayer[player] = instance;
        return instance;
    }

    /// <summary>
    /// Removes and destroys the UI_PlayerStats for the given player. Call when a player disconnects.
    /// </summary>
    public void RemovePlayer(Player player)
    {
        if (player == null) return;

        if (!_playerStatsByPlayer.TryGetValue(player, out var stats))
            return;

        _playerStatsByPlayer.Remove(player);
        if (stats != null && stats.gameObject != null)
            Destroy(stats.gameObject);
    }

    /// <summary>
    /// Spawns a PickupHUD that follows a world-space transform, converting it to display space each frame.
    /// </summary>
    /// <param name="worldTransform">The transform in world space to follow (e.g. item or pickup point).</param>
    /// <param name="player">Optional. The player whose color is used for the HUD background.</param>
    /// <param name="sprite">Optional. If set, applies the sprite to the item image.</param>
    /// <returns>The created UI_PickupHUD instance, or null if prefab not set.</returns>
    public UI_PickupHUD SpawnPickupHUD(Transform worldTransform, Sprite sprite = null, Player player = null)
    {
        if (worldTransform == null || pickupHudPrefab == null)
            return null;

        Transform parent = pickupHudContainer != null ? pickupHudContainer : transform;
        var instance = Instantiate(pickupHudPrefab, parent);
        instance.Initialize(worldTransform, sprite, player);
        return instance;
    }

    /// <summary>
    /// Refreshes all visible player stat panels (e.g. money and shopping list counts). Call when any player's data changes.
    /// </summary>
    public void RefreshAllPlayers()
    {
        foreach (var stats in _playerStatsByPlayer.Values)
        {
            if (stats != null)
                stats.Refresh();
        }
    }
}
