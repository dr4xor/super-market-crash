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
    }
}
