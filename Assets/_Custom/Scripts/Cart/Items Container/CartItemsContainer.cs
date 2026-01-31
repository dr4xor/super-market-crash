using System.Collections.Generic;
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

    private List<ItemFacade> _itemsInCart = new List<ItemFacade>();

    public void InstantiateAndAddItemToCart()
    {
        var goItem = Instantiate(prefabItemExample);
        goItem.transform.position = spawnOfExampleItem.position;
        AddItemToCart(goItem);
    }   
    
    public void AddItemToCart(ItemFacade item)
    {
        setLayerRecursively(item.gameObject, LayerMask.NameToLayer(layerOfItems));
        item.GetComponent<Rigidbody>().isKinematic = true;
        CartFlyingItem cartFlyingItem = item.gameObject.AddComponent<CartFlyingItem>();
        cartFlyingItem.StartFlying(flyTime, yHeightFlyCurve, posOfItemToFlyTo);
        
        cartFlyingItem.OnFlyingFinished += OnFlyingFinished;
    }

    private void OnFlyingFinished(CartFlyingItem cartFlyingItem)
    {
        Rigidbody rb = cartFlyingItem.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        rb.linearVelocity = velocityWhenFlyingFinished;
        _itemsInCart.Add(cartFlyingItem.GetComponent<ItemFacade>());

        Destroy(cartFlyingItem);

        Debug.Log("Flying finished");
    }

    private void Update()
    {
        
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
