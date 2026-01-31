using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Supermarket
{
    public class ShelfController : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private List<Transform> shelfItemPositions;
        [SerializeField] private  ItemFacade itemPrefab;
        
        public ItemTemplate itemTemplate;
        public int shelfItemCount;
        
        private readonly List<ItemFacade> _shelfItems = new();
        
        private void Start()
        {
            image.sprite = itemTemplate.sprite;
            for (var i = 0; i < shelfItemCount; i++)
            {
                var item = Instantiate(itemPrefab, shelfItemPositions[i].position, Quaternion.identity);
                item.Init(itemTemplate);
                _shelfItems.Add(item);  
            }
        }
        
        public int RemainingItems => _shelfItems.Count;

        public bool TryTakeItem(out ItemFacade item)
        {
            item = null;
            if (!_shelfItems.Any())
            {
                return false;
            }

            item = _shelfItems.First();
            return true;
        }
    }
}
