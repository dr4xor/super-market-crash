using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using UnityEngine;

namespace Scripts.Supermarket
{
    public class ShelfController : MonoBehaviour
    {
        [SerializeField] private List<Transform> shelfItemPositions;
        [SerializeField] private ItemTemplate itemTemplate;

        public int shelfItemCount;
        
        private Dictionary<Transform, ItemTemplate> _shelfItems = new();
        
        private void Start()
        {
            for (var i = 0; i < shelfItemPositions.Count; i++)
            {
              _shelfItems.Add(shelfItemPositions[i], itemTemplate);  
            }  
        }
        
        public bool TryGetItemTemplate(out ItemTemplate item)
        {
            var shelfPosition = _shelfItems.Keys.First();
            return _shelfItems.TryGetValue(shelfPosition, out item);
        }
    }
}
