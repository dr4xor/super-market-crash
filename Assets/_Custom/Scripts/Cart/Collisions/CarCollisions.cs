using Scripts.Supermarket;
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
        if (collision.rigidbody != null)
        {
            Cart cart = collision.rigidbody.GetComponent<Cart>();
            ItemFacade itemFacade = collision.rigidbody.GetComponent<ItemFacade>();

            if (cart != null)
            {
                _cartCollisionHandler.CollisionWithOtherCart(cart.GetComponent<CartCollisionHandler>());
            }
            else if (itemFacade != null)
            {

            }
            else
            {
                _cartCollisionHandler.CollisionWithSomething();
            }
        }
        else
        {
            _cartCollisionHandler.CollisionWithSomething();
        }
    }
}
