using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ShoppingListItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject checkmarkOverlay;
    [SerializeField] private TextMeshProUGUI amountText;

    private ItemTemplate _itemTemplate;
    private int _needed;
    private int _collected;

    /// <summary>
    /// Binds this list item to an item type and amounts.
    /// </summary>
    /// <param name="template">The item template (icon, name, etc.).</param>
    /// <param name="needed">Required quantity.</param>
    /// <param name="collected">Current collected quantity.</param>
    public void SetItem(ItemTemplate template, int needed, int collected)
    {
        _itemTemplate = template;
        _needed = needed;
        _collected = collected;

        Refresh();
    }

    /// <summary>
    /// Updates only the collected amount (e.g. when player picks up items).
    /// Call after SetItem when you don't want to change template/needed.
    /// </summary>
    public void SetCollected(int collected)
    {
        _collected = collected;
        Refresh();
    }

    private void Refresh()
    {
        if (_itemTemplate == null) return;

        if (iconImage != null && _itemTemplate.sprite != null)
        {
            iconImage.sprite = _itemTemplate.sprite;
            iconImage.enabled = true;
        }
        else if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        if (amountText != null)
        {
            if (_needed <= 1)
                amountText.gameObject.SetActive(false);
            else
            {
                amountText.gameObject.SetActive(true);
                amountText.text = $"{_collected} / {_needed}";
            }
        }

        if (checkmarkOverlay != null)
            checkmarkOverlay.SetActive(_collected >= _needed);
    }
}
