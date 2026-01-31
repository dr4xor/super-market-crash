using UnityEngine;

public class CartCollisionHandler : MonoBehaviour
{
    [SerializeField] private float minVelocityForHit = 8f;

    private Cart _cart;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _cart = GetComponent<Cart>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void CollisionWithOtherCart(CartCollisionHandler otherCartCollisionHandler)
    {
        if (otherCartCollisionHandler._rigidbody.linearVelocity.magnitude > _rigidbody.linearVelocity.magnitude)
        {
            return;
        }

        Debug.Log("Cur self velocity: " + _rigidbody.linearVelocity.magnitude);

        if (_rigidbody.linearVelocity.magnitude < minVelocityForHit)
        {
            return;
        }
        
        _cart.CartShaker.ShakeDueToCrash();

        Debug.Log("Collision with other cart");
    }
}
