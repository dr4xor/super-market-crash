using System;
using System.Collections.Generic;
using Scripts.Supermarket;
using UnityEngine;

public class RestockingManager : MonoBehaviour
{
    private static RestockingManager _instance;
    public static RestockingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<RestockingManager>();
            }
            return _instance;
        }
    }

    private List<ShelfFacade> _shelves = new();
    private List<NPCController> _restockers = new();

    private List<ShelfFacade> _shelvesScheduledForRestocking = new();
    private List<NPCController> _npcsCurrentlyRestocking = new();
    private List<ShelfFacade> _shelvesCurrentlyRestocking = new();

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        //checkForRestocking();
    }

    public void AddShelf(ShelfFacade shelf)
    {
        _shelves.Add(shelf);
        shelf.OnShelfItemsChange += OnShelfItemsChange;
    }

    public void RegisterRestocker(NPCController restocker)
    {
        _restockers.Add(restocker);
        restocker.OnStateChanged += OnRestockerStateChanged;
    }

    private void OnRestockerStateChanged(NPCController npc, NPCState newState)
    {
        if (newState == NPCState.AT_ORIGIN)
        {
            checkForRestocking();
        }
        else if (newState == NPCState.AT_TARGET)
        {
            for (int i = 0; i < _npcsCurrentlyRestocking.Count; i++)
            {
                NPCController restocker = _npcsCurrentlyRestocking[i];
                if (restocker == npc)
                {
                    ShelfFacade shelf = _shelvesCurrentlyRestocking[i];
                    shelf.Restock();

                    _npcsCurrentlyRestocking.RemoveAt(i);
                    _shelvesCurrentlyRestocking.RemoveAt(i);
                }
            }
        }
    }

    private void OnShelfItemsChange(ShelfFacade shelf, List<ItemFacade> items)
    {
        if (items.Count <= 0)
        {
            _shelvesScheduledForRestocking.Add(shelf);
        }
    }

    private void checkForRestocking()
    {
        if (_shelvesScheduledForRestocking.Count <= 0)
        {
            return;
        }

        ShelfFacade shelf = _shelvesScheduledForRestocking[0];

        for (int i = 0; i < _restockers.Count; i++)
        {
            NPCController restocker = _restockers[i];
            if (restocker.CurrentState != NPCState.AT_ORIGIN)
            {
                continue;
            }

            _npcsCurrentlyRestocking.Add(restocker);
            _shelvesCurrentlyRestocking.Add(shelf);
            _shelvesScheduledForRestocking.Remove(shelf);
            restocker.GoToTarget(shelf.RestockingPosition, 3f, "isRestocking");
        }
    }
}
