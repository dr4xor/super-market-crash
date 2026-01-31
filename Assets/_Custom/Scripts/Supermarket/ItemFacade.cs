using ScriptableObjects;
using UnityEngine;

public class ItemFacade : MonoBehaviour
{
    public void Init(ItemTemplate template)
    {
        Instantiate(template.itemPrefab, transform);
    }
}
