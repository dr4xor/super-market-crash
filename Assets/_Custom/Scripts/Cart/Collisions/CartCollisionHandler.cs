using ScriptableObjects;
using Scripts.Supermarket;
using UnityEngine;

public class CartCollisionHandler : MonoBehaviour
{
    [SerializeField] private float minVelocityForHit = 8f;
    [SerializeField] private CartCrashConfig cartCrashConfig;

    private Cart _cart;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _cart = GetComponent<Cart>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void CollisionWithOtherCart(CartCollisionHandler otherCartCollisionHandler)
    {
        float selfVelocity = _cart.MovingAverageVelocity.GetValueInPast(0.2f);
        float otherVelocity = otherCartCollisionHandler._cart.MovingAverageVelocity.GetValueInPast(0.2f);

        if (otherVelocity > selfVelocity)
        {
            return;
        }

        Debug.Log("Cur self velocity: " + selfVelocity);

        if (selfVelocity < minVelocityForHit)
        {
            return;
        }
        
        otherCartCollisionHandler._cart.CartShaker.ShakeDueToCrash();
        
        int amountOfItemsToLose = cartCrashConfig.ComputeAmountOfItemstoLose(
            otherCartCollisionHandler._cart.CartItemsContainer.ItemsInCart.Count,
            selfVelocity,
            true);

        otherCartCollisionHandler._cart.CartItemsContainer.LoseItems(amountOfItemsToLose);
        Debug.Log("Collision with other cart");
    }

    public void CollisionWithSomething()
    {
        float selfVelocity = _cart.MovingAverageVelocity.GetValueInPast(0.2f);

        int amountOfItemsToLose = cartCrashConfig.ComputeAmountOfItemstoLose(
            _cart.CartItemsContainer.ItemsInCart.Count,
            selfVelocity,
            false);

        _cart.CartItemsContainer.LoseItems(amountOfItemsToLose);
    }
}
