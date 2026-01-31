using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/ItemDatabase")]
    public class ItemDatabase : ScriptableObject
    {
        public ItemTemplate[] items;
    }
}
