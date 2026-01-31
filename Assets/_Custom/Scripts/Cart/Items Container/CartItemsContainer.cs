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

    private List<ItemFacade> _itemsInCart = new List<ItemFacade>();
    private List<float> _freezeItemsIn = new List<float>();

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
    }

    private void handleFreezeItems()
    {
        for (int i = 0; i < _freezeItemsIn.Count; i++)
        {
            if (_freezeItemsIn[i] > 0)
            {
                _freezeItemsIn[i] -= Time.deltaTime;

                if (_freezeItemsIn[i] <= 0)
                {
                    _itemsInCart[i].transform.parent = transform;
                    _itemsInCart[i].GetComponent<Rigidbody>().isKinematic = true;
                    Destroy(_itemsInCart[i].GetComponent<Rigidbody>());
                }
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

    private void setLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
        {
            setLayerRecursively(child.gameObject, layer);
        }
    }
}
