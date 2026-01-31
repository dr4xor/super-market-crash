using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Scripts.Supermarket;
using UnityEngine;

public class PlayerCheckoutInteractor : MonoBehaviour
{
    [SerializeField] private CartItemsContainer itemsContainer;
    [SerializeField] private Sprite checkoutSprite;
    
    private CheckoutFacade _checkoutInRange;
    private Player _player;
    private UI_PickupHUD _pickupHud;
    private TweenerCore<float, float, FloatOptions> _tween;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CheckoutFacade colliderCheckout))
        {
            _checkoutInRange = colliderCheckout;
            if (!_pickupHud && itemsContainer.ItemsInCart.Count > 0)
            {
                _pickupHud = UI_Manager.Instance.SpawnPickupHUD(_checkoutInRange.hudPosition, checkoutSprite, _player);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CheckoutFacade _))
        {
            _checkoutInRange = null;
            if (_pickupHud)
            {
                Destroy(_pickupHud.gameObject);
            }
        }
    }

    public void OnInteract()
    {
        if (_checkoutInRange)
        {
            _tween = DOTween.To(() => 0f, x => _pickupHud.SetProgress(x), 1f, 2f)
                .SetEase(Ease.Linear)
                .SetLink(_pickupHud.gameObject)
                .OnComplete(() =>
                {
                    var cost = itemsContainer.ItemsInCart.Sum(item => item.ItemTemplate.price);
                    if (_player.Money >= cost)
                    {
                        _player.Money -= cost;
                        itemsContainer.ClearCartAndSetAllItemsAsBought();
                        Destroy(_pickupHud.gameObject);
                        _checkoutInRange = null;
                    }
                });
        }
    }
    
    public void OnInteractCancel()
    {
        if (_pickupHud)
        {
            _tween.Kill();
            _pickupHud.SetProgress(0f);
        }
    }
}