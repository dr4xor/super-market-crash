using System;
using System.Collections.Generic;
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
}
