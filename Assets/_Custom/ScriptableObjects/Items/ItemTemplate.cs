using System.Linq;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "ItemTemplate", menuName = "Scriptable Objects/ItemTemplate")]
    public class ItemTemplate : ScriptableObject
    {
        public string itemName;
        public GameObject itemPrefab;
        public Sprite sprite;
        public int price;
        public int weight;
        public ShelfType[] shelfTypes;

        public bool CanFitInShelf(ShelfType shelfType)
        {
            if (shelfTypes == null
                || shelfTypes.Length == 0)
            {
                return shelfType == ShelfType.Shelf;
            }

            return shelfTypes.Contains(shelfType);
        }
    }

    public enum ShelfType
    {
        Shelf,
        Freezer,
        Board,
        VegetableBoxes,
    }
}
