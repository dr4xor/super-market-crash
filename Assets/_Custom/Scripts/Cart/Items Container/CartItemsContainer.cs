using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

public class CartItemsContainer : MonoBehaviour
{
    [SerializeField] private ItemFacade prefabItemExample;
    [SerializeField] private Transform spawnOfExampleItem;
    [SerializeField] private Transform posOfItemToFlyTo;
    [SerializeField] private AnimationCurve yHeightFlyCurve;
    [SerializeField] private float flyTime = 0.5f;
    [SerializeField] private Vector3 velocityWhenFlyingFinished = new Vector3(0f, -3f, 0f);
    [SerializeField] private string layerOfItems;
    [SerializeField] private float timeUntilFreezeItems = 0.5f;
    [SerializeField] private ItemTemplate[] itemTemplates;
    [SerializeField] private float dragWhenInCart;
    [SerializeField] private float angDragWhenInCart;
    [SerializeField] private Transform areaItemLose;
    [SerializeField] private float flyAwaySpeed;
    [SerializeField] private float flyAwayAngularSpeed;
    [SerializeField] private float freezeVelocityThreshold = 0.5f;
    [SerializeField] private float timeForVelocityUnderThreshold = 0.5f;
    [SerializeField] private Transform areaOutsideCart;

    private List<ItemFacade> _itemsInCart = new List<ItemFacade>();
    private List<float> _freezeItemsIn = new List<float>();

    public List<ItemFacade> ItemsInCart => _itemsInCart;

    private Player _player;

    private void Start()
    {
        _player = GetComponentInParent<Player>();
    }

    public void InstantiateAndAddItemToCart()
    {
        var goItem = Instantiate(prefabItemExample);
        goItem.transform.localScale = Vector3.one * 0.17f;
        goItem.Init(itemTemplates[Random.Range(0, itemTemplates.Length)]);
        goItem.transform.position = spawnOfExampleItem.position;
        AddItemToCart(goItem);
    }

    public void ClearCartAndSetAllItemsAsBought()
    {
        copyAllItemsInCartToBoughtItems();

        _itemsInCart.Clear();
        _freezeItemsIn.Clear();

        updatePlayerItemsInCartData();

        for (int i = 0; i < _itemsInCart.Count; i++)
        {
            Destroy(_itemsInCart[i].gameObject, Random.Range(0.0f, 0.7f));
        }
    }

    public void LoseItems(int amountOfItemsToLose)
    {
        for (int i = 0; i < amountOfItemsToLose; i++)
        {
            int randomIndex = Random.Range(0, _itemsInCart.Count);
            ItemFacade itemToLose = _itemsInCart[randomIndex];
            _itemsInCart.RemoveAt(randomIndex);
            _freezeItemsIn.RemoveAt(randomIndex);

            itemToLose.gameObject.AddComponent<Rigidbody>();
            
            itemToLose.transform.position = getRandomPositionInArea();
            itemToLose.transform.parent = null;
            itemToLose.GetComponent<Rigidbody>().isKinematic = false;
            itemToLose.GetComponent<Rigidbody>().linearVelocity = getRandomUpVector() * flyAwaySpeed;
            itemToLose.GetComponent<Rigidbody>().angularVelocity = getRandomAngularVelocity() * flyAwayAngularSpeed
                * UnityEngine.Random.Range(0.5f, 1f);
            itemToLose.GetComponent<Rigidbody>().mass = 0.01f;
            itemToLose.GetComponent<Rigidbody>().linearDamping = dragWhenInCart;
            itemToLose.GetComponent<Rigidbody>().angularDamping = angDragWhenInCart;

            setLayerRecursively(itemToLose.gameObject, LayerMask.NameToLayer("Default"));
        }

        updatePlayerItemsInCartData();
    }


    public void AddItemToCart(ItemFacade item)
    {
        item.GetComponent<Rigidbody>().isKinematic = true;
        CartFlyingItem cartFlyingItem = item.gameObject.AddComponent<CartFlyingItem>();
        cartFlyingItem.StartFlying(flyTime, yHeightFlyCurve, posOfItemToFlyTo);
        cartFlyingItem.OnFlyingFinished += OnFlyingFinished;
    }

    private void OnFlyingFinished(CartFlyingItem cartFlyingItem)
    {
        setLayerRecursively(cartFlyingItem.gameObject, LayerMask.NameToLayer(layerOfItems));

        Rigidbody rb = cartFlyingItem.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.linearDamping = dragWhenInCart;
        rb.angularDamping = angDragWhenInCart;
        rb.mass = 0.01f;

        rb.linearVelocity = velocityWhenFlyingFinished;
        _itemsInCart.Add(cartFlyingItem.GetComponent<ItemFacade>());
        _freezeItemsIn.Add(timeUntilFreezeItems);

        updatePlayerItemsInCartData();


        Destroy(cartFlyingItem);

        Debug.Log("Flying finished");
    }

    private void Update()
    {
        handleFreezeItems();
        checkIfItemsAreOutsideCart();
    }

    private void handleFreezeItems()
    {
        for (int i = 0; i < _freezeItemsIn.Count; i++)
        {
            /// Is already freezed
            if (_freezeItemsIn[i] <= 0f)
            {
                continue;
            }

            if (_itemsInCart[i].GetComponent<Rigidbody>().linearVelocity.magnitude < freezeVelocityThreshold)
            {
                _freezeItemsIn[i] -= Time.deltaTime;
                if (_freezeItemsIn[i] <= 0)
                {
                    _itemsInCart[i].transform.parent = transform;
                    _itemsInCart[i].GetComponent<Rigidbody>().isKinematic = true;
                    Destroy(_itemsInCart[i].GetComponent<Rigidbody>());
                }
            }
            else
            {
                _freezeItemsIn[i] = timeForVelocityUnderThreshold;
            }
        }
    }

    private void updatePlayerItemsInCartData()
    {
        _player.ItemsInCart.Clear();
        for (int i = 0; i < _itemsInCart.Count; i++)
        {
            if (_player.ItemsInCart.ContainsKey(_itemsInCart[i].ItemTemplate))
            {
                _player.ItemsInCart[_itemsInCart[i].ItemTemplate] += 1;
            }
            else
            {
                _player.ItemsInCart[_itemsInCart[i].ItemTemplate] = 1;
            }
        }
    }

    private void copyAllItemsInCartToBoughtItems()
    {
        for (int i = 0; i < _itemsInCart.Count; i++)
        {
            if (_player.BoughtItems.ContainsKey(_itemsInCart[i].ItemTemplate))
            {
                _player.BoughtItems[_itemsInCart[i].ItemTemplate] += 1;
            }
            else
            {
                _player.BoughtItems[_itemsInCart[i].ItemTemplate] = 1;
            }
        }
    }

    private void setLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
        {
            setLayerRecursively(child.gameObject, layer);
        }
    }

    private Vector3 getRandomPositionInArea()
    {
        return new Vector3(Random.Range(areaItemLose.position.x - areaItemLose.localScale.x / 2, areaItemLose.position.x + areaItemLose.localScale.x / 2),
            Random.Range(areaItemLose.position.y - areaItemLose.localScale.y / 2, areaItemLose.position.y + areaItemLose.localScale.y / 2),
            Random.Range(areaItemLose.position.z - areaItemLose.localScale.z / 2, areaItemLose.position.z + areaItemLose.localScale.z / 2));
    }

    private Vector3 getRandomUpVector()
    {
        return new Vector3(Random.Range(-1f, 1f), 1.5f, Random.Range(-1f, 1f)).normalized;
    }

    private Vector3 getRandomAngularVelocity()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(areaItemLose.position, areaItemLose.localScale);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(areaOutsideCart.position, areaOutsideCart.localScale);
    }

    private void checkIfItemsAreOutsideCart()
    {
        for (int i = 0; i < _itemsInCart.Count; i++)
        {
            if (_freezeItemsIn[i] <= 0f)
            {
                continue;
            }
            
            Vector3 localPos = areaItemLose.InverseTransformPoint(_itemsInCart[i].transform.position);
            if (localPos.x < -0.5f || localPos.x > 0.5f || localPos.z < -0.5f || localPos.z > 0.5f)
            {
                _itemsInCart[i].transform.position = posOfItemToFlyTo.position;
                _itemsInCart[i].GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            }
        }
    }
}
