using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "CartCrashConfig", menuName = "Scriptable Objects/CartCrashConfig")]
    public class CartCrashConfig : ScriptableObject
    {
        public AnimationCurve speedDiffToDamageCurve;
        public AnimationCurve itemsCountToVulnerabilityCurve;
        public float reductionIfNotAgainstOtherCart = 0.5f;

        public int ComputeAmountOfItemstoLose(int itemsInCart, float speedDiff, bool isAgainstOtherCart)
        {
            float damageCaused = speedDiffToDamageCurve.Evaluate(speedDiff);
            float vulnerability = itemsCountToVulnerabilityCurve.Evaluate(itemsInCart);
            float amountOfItemsToLose = damageCaused * vulnerability * (isAgainstOtherCart ? 1f : reductionIfNotAgainstOtherCart);
            
            if (isAgainstOtherCart)
            {
                Debug.Log("Cart collision. Speed diff: " + speedDiff + ", vulnerability: " + vulnerability + ", amount of items to lose: " + amountOfItemsToLose);
            }
            else
            {
                Debug.Log("Cart collision with something. Speed diff: " + speedDiff + ", vulnerability: " + vulnerability + ", amount of items to lose: " + amountOfItemsToLose);
            }
            
            
            return Mathf.RoundToInt(amountOfItemsToLose);
        }
    }
}