using ScriptableObjects;
using UnityEngine;

public class ItemFacade : MonoBehaviour
{

    public ItemTemplate ItemTemplate { get; private set; }
    public void Init(ItemTemplate template)
    {
        Instantiate(template.itemPrefab, transform);
        ItemTemplate = template;
    }
}
