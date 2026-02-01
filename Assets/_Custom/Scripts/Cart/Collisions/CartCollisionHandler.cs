using ScriptableObjects;
using Scripts.Supermarket;
using UnityEngine;

public class CartCollisionHandler : MonoBehaviour
{
    [SerializeField] private float minVelocityForHit = 8f;
    [SerializeField] private CartCrashConfig cartCrashConfig;
    [SerializeField] private AudioSource audioSourceCartCollision;
    [SerializeField] private AudioSource audioSourceHurtPlayer;
    [SerializeField] private AudioClip clipCartCollision;
    [SerializeField] private AudioClip[] clipHurtPlayer;

    private Cart _cart;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _cart = GetComponent<Cart>();
        _rigidbody = GetComponent<Rigidbody>();
        audioSourceCartCollision.clip = clipCartCollision;
    }

    public void CollisionWithOtherCart(CartCollisionHandler otherCartCollisionHandler)
    {
        float selfVelocity = _cart.MovingAverageVelocity.GetValueInPast(0.2f);
        float otherVelocity = otherCartCollisionHandler._cart.MovingAverageVelocity.GetValueInPast(0.2f);

        _cart.ResetCurrentDashFactor();


        if (otherVelocity > selfVelocity)
        {
            return;
        }

        Debug.Log("Cur self velocity: " + selfVelocity);

        //audioSourceCartCollision.Play();

        if (selfVelocity < minVelocityForHit)
        {
            return;
        }



        otherCartCollisionHandler.audioSourceHurtPlayer.clip = clipHurtPlayer[Random.Range(0, clipHurtPlayer.Length)];
        otherCartCollisionHandler.audioSourceHurtPlayer.Play();

        otherCartCollisionHandler._cart.GetComponent<PlayerAnimationController>().PlayBeingHitAnimation();



        int amountOfItemsToLose = cartCrashConfig.ComputeAmountOfItemstoLose(
            otherCartCollisionHandler._cart.CartItemsContainer.ItemsInCart.Count,
            selfVelocity,
            true);

        if (amountOfItemsToLose > 0)
        {
            otherCartCollisionHandler._cart.CartShaker.ShakeDueToCrash();
        }

        otherCartCollisionHandler._cart.CartItemsContainer.LoseItems(amountOfItemsToLose);
        Debug.Log("Collision with other cart");
    }

    public void CollisionWithSomething()
    {
        _cart.ResetCurrentDashFactor();


        //audioSourceCartCollision.Play();

        float selfVelocity = _cart.MovingAverageVelocity.GetValueInPast(0.2f);

        int amountOfItemsToLose = cartCrashConfig.ComputeAmountOfItemstoLose(
            _cart.CartItemsContainer.ItemsInCart.Count,
            selfVelocity,
            false);

        if (amountOfItemsToLose > 0)
        {
            _cart.CartShaker.ShakeDueToCrash();
        }

        _cart.CartItemsContainer.LoseItems(amountOfItemsToLose);
    }

    public void CollisionWithCashier(NPCController npc)
    {
        int amountOfItemsToLose = cartCrashConfig.ComputeAmountOfItemsToLoseForCashier(
            _cart.CartItemsContainer.ItemsInCart.Count,

            npc.VelocityMovingAverage.GetValueInPast(0.1f));

        _cart.CartItemsContainer.LoseItems(amountOfItemsToLose);

        if (amountOfItemsToLose > 0)
        {
            _cart.CartShaker.ShakeDueToCrash();
        }
    }
}
