using System;
using System.Collections.Generic;
using ScriptableObjects;
using Scripts.Supermarket;
using UnityEngine;

public class ShelfsManager : MonoBehaviour
{
    private static ShelfsManager _instance;
    public static ShelfsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<ShelfsManager>();
            }
            return _instance;
        }
    }
    
    private List<ShelfFacade> _shelves = new();
    public List<ShelfFacade> Shelves => _shelves;

    private void Awake()
    {
        _instance = this;
    }

    public void AddShelf(ShelfFacade shelf)
    {
        _shelves.Add(shelf);
    }

    public List<ItemTemplate> GetAllItemTemplates()
    {
        List<ItemTemplate> itemTemplates = new();
        for (int i = 0; i < _shelves.Count; i++)
        {
            if (itemTemplates.Contains(_shelves[i].itemTemplate))
                continue;
            itemTemplates.Add(_shelves[i].itemTemplate);
        }
        return itemTemplates;
    }
}
