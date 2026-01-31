using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Supermarket
{
    public class ShelfFacade : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private List<Transform> shelfItemPositions;
        [SerializeField] private ItemFacade itemPrefab;
        [SerializeField] private Transform restockingPosition;
        [SerializeField] private ItemDatabase itemDatabase;

        public Transform hudPosition;
        public ItemTemplate itemTemplate;
        public int shelfItemCount;
        public ShelfType shelfType;
        public bool shouldSpawnRandomItem;


        private readonly List<ItemFacade> _shelfItems = new();

        public delegate void ShelfItemsChangeEvent(ShelfFacade shelf, List<ItemFacade> items);
        public event ShelfItemsChangeEvent OnShelfItemsChange;

        public Transform RestockingPosition => restockingPosition;

        private void Awake()
        {
            ShelfsManager.Instance.AddShelf(this);
        }

        private void Start()
        {
            if (shouldSpawnRandomItem)
            {
                itemTemplate = getRandomItemTemplate();
            }

            image.sprite = itemTemplate.sprite;
            Restock();
        }


        public bool HasItems => _shelfItems.Any();

        public bool TryTakeItem(out ItemFacade item)
        {
            item = null;
            if (!_shelfItems.Any())
            {
                return false;
            }

            item = _shelfItems.First();
            _shelfItems.Remove(item);
            OnShelfItemsChange?.Invoke(this, _shelfItems);
            return true;
        }

        public void Restock()
        {
            Debug.Log("Restocking shelf: " + name);
            for (var i = 0; i < shelfItemCount; i++)
            {
                var item = Instantiate(itemPrefab, shelfItemPositions[i].position, Quaternion.identity);
                item.Init(itemTemplate);
                _shelfItems.Add(item);
                Debug.Log("Added item to shelf: " + item.name);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < shelfItemPositions.Count; i++)
            {
                Gizmos.DrawSphere(shelfItemPositions[i].position, 0.1f);
            }
        }

        [ContextMenu("Clear And Restock")]
        private void clearAndRestock()
        {
            foreach (var item in _shelfItems)
            {
                Destroy(item.gameObject);
            }
            _shelfItems.Clear();
            Restock();
        }

        private ItemTemplate getRandomItemTemplate()
        {
            ItemTemplate[] itemTemplates = itemDatabase.items.Where(item => item.CanFitInShelf(shelfType)).ToArray();
            return itemTemplates[Random.Range(0, itemTemplates.Length)];
        }
    }
}
