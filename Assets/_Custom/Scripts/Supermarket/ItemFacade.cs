using ScriptableObjects;
using UnityEngine;

public class ItemFacade : MonoBehaviour
{
    private ItemTemplate _template;
    
    public void Init(ItemTemplate template)
    {
        _template = template;
    }
}
