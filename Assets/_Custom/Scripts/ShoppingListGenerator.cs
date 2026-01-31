using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

public static class ShoppingListGenerator
{
    private const int ItemTypeCount = 6;
    private const int TotalItemCount = 20;

    /// <summary>
    /// Generates a random shopping list.
    /// </summary>
    public static Dictionary<ItemTemplate, int> Generate(List<ItemTemplate> availableItems)
    {
        var result = new Dictionary<ItemTemplate, int>();

        if (availableItems == null || availableItems.Count == 0)
            return result;

        // Filter valid items
        var validItems = new List<ItemTemplate>();
        foreach (var item in availableItems)
        {
            if (item != null)
                validItems.Add(item);
        }

        if (validItems.Count == 0)
            return result;

        // Pick 6 random unique item types (or less if not enough available)
        int typesToPick = Mathf.Min(ItemTypeCount, validItems.Count);
        var selectedItems = new List<ItemTemplate>();

        var shuffled = new List<ItemTemplate>(validItems);
        for (int i = 0; i < typesToPick; i++)
        {
            int randomIndex = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[randomIndex]) = (shuffled[randomIndex], shuffled[i]);
            selectedItems.Add(shuffled[i]);
        }

        // Start each item type with 1 item
        foreach (var item in selectedItems)
        {
            result[item] = 1;
        }

        // Distribute remaining items randomly
        int remaining = TotalItemCount - typesToPick;
        for (int i = 0; i < remaining; i++)
        {
            var randomItem = selectedItems[Random.Range(0, selectedItems.Count)];
            result[randomItem]++;
        }

        return result;
    }
}
