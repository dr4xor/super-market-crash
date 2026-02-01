using ScriptableObjects;
using Scripts.Supermarket;
using UnityEngine;

public class CarCollisions : MonoBehaviour
{
    [SerializeField] private CartCrashConfig cartCrashConfig;

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

            NPCController npc = collision.collider.GetComponent<NPCController>();

            if (npc != null
                && npc.VelocityMovingAverage.GetValueInPast(0.1f) > cartCrashConfig.minCashierSpeedForDamage)
            {
                _cartCollisionHandler.CollisionWithCashier(npc);
            }
            else
            {
                _cartCollisionHandler.CollisionWithSomething();
            }
        }
    }
}
