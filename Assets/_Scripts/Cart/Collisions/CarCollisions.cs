using UnityEngine;

public class CarCollisions : MonoBehaviour
{
    private CartCollisionHandler _cartCollisionHandler;

    private void Start()
    {
        _cartCollisionHandler = GetComponentInParent<CartCollisionHandler>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Cart cart = collision.rigidbody.GetComponent<Cart>();

        Debug.Log("Collision with other cart. Cart is null: " + (cart == null).ToString());

        if (cart != null)
        {
            _cartCollisionHandler.CollisionWithOtherCart(cart.GetComponent<CartCollisionHandler>());
        }
    }
}
