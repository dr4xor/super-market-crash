using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

public static class ShoppingListGenerator
{
    private const int TargetValue = 20;
    private const int MinValue = 16;
    private const int MaxValue = 24;

    /// <summary>
    /// Generates a random shopping list from the database with a total value of approximately 20 money.
    /// </summary>
    public static Dictionary<ItemTemplate, int> Generate(ItemDatabase database)
    {
        var result = new Dictionary<ItemTemplate, int>();
        if (database == null || database.items == null || database.items.Length == 0)
            return result;

        var affordableItems = new List<ItemTemplate>();
        foreach (var item in database.items)
        {
            if (item != null && item.price > 0 && item.price <= MaxValue)
                affordableItems.Add(item);
        }

        if (affordableItems.Count == 0)
            return result;

        int totalValue = 0;
        int attempts = 0;
        const int maxAttempts = 100;

        while (totalValue < MinValue && attempts < maxAttempts)
        {
            var item = affordableItems[Random.Range(0, affordableItems.Count)];
            int potentialCost = item.price;
            if (totalValue + potentialCost <= MaxValue)
            {
                if (result.TryGetValue(item, out var qty))
                    result[item] = qty + 1;
                else
                    result[item] = 1;
                totalValue += potentialCost;
            }
            attempts++;
        }

        return result;
    }
}
