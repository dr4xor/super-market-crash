using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_PlayerStats : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Transform shoppingListContainer;
    [SerializeField] private UI_ShoppingListItem shoppingListItemPrefab;

    private Player _player;
    private readonly List<UI_ShoppingListItem> _listItems = new();

    /// <summary>
    /// Binds this panel to a player. Updates background color, money, and rebuilds the shopping list.
    /// </summary>
    public void Bind(Player player)
    {
        _player = player;
        if (_player == null) return;

        RefreshAll();
    }

    /// <summary>
    /// Sets the displayed player name (e.g. from your connection/player setup script).
    /// </summary>
    public void SetPlayerName(string playerName)
    {
        if (playerNameText != null)
            playerNameText.text = playerName;
    }

    /// <summary>
    /// Refreshes money display and all shopping list item counts. Call when the player's cart or money changes.
    /// </summary>
    public void Refresh()
    {
        if (_player == null) return;

        if (moneyText != null)
            moneyText.text = _player.Money.ToString();

        RefreshShoppingListCollected();
    }

    /// <summary>
    /// Rebuilds the entire UI from the bound player (color, money, list). Use after Bind or when ShoppingList content changes.
    /// </summary>
    public void RefreshAll()
    {
        if (_player == null) return;

        if (backgroundImage != null)
            backgroundImage.color = _player.Color;

        Refresh();
        RebuildShoppingList();
    }

    private void RebuildShoppingList()
    {
        if (shoppingListContainer == null || shoppingListItemPrefab == null || _player == null) return;

        ClearShoppingList();

        foreach (var kvp in _player.ShoppingList)
        {
            var template = kvp.Key;
            var needed = kvp.Value;
            var collected = GetCollectedCount(template);

            var item = Instantiate(shoppingListItemPrefab, shoppingListContainer);
            item.SetItem(template, needed, collected);
            _listItems.Add(item);
        }
    }

    private void RefreshShoppingListCollected()
    {
        if (_player == null) return;

        int index = 0;
        foreach (var kvp in _player.ShoppingList)
        {
            if (index >= _listItems.Count) break;
            var collected = GetCollectedCount(kvp.Key);
            _listItems[index].SetCollected(collected);
            index++;
        }
    }

    private int GetCollectedCount(ItemTemplate template)
    {
        if (_player == null) return 0;
        _player.ItemsInCart.TryGetValue(template, out var inCart);
        _player.BoughtItems.TryGetValue(template, out var bought);
        return inCart + bought;
    }

    private void ClearShoppingList()
    {
        foreach (var item in _listItems)
        {
            if (item != null && item.gameObject != null)
                Destroy(item.gameObject);
        }
        _listItems.Clear();
    }
}
