using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using UnityEngine;

namespace Scripts.Supermarket
{
    public class ShelfController : MonoBehaviour
    {
        [SerializeField] private List<Transform> shelfItemPositions;
        
        public ItemTemplate itemTemplate;
        public int shelfItemCount;
        
        private List<GameObject> _shelfItems = new();
        
        private void Start()
        {
            for (var i = 0; i < shelfItemCount; i++)
            {
                var itemVisual = Instantiate(itemTemplate.itemPrefab, shelfItemPositions[i].position, Quaternion.identity);
                _shelfItems.Add(itemVisual);  
            }
        }
        
        public int RemainingItems => _shelfItems.Count;

        public bool TryTakeItem(out ItemTemplate item)
        {
            item = itemTemplate;
            return _shelfItems.Any();
        }
    }
}
