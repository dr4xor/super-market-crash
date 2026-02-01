using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Scripts.Supermarket;
using UnityEngine;

public class PlayerShelfInRangeChecker : MonoBehaviour
{
    public readonly HashSet<ShelfFacade> ShelvesInRange = new ();

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ShelfFacade colliderShelf))
        {
            ShelvesInRange.Add(colliderShelf);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ShelfFacade colliderShelf))
        {
            ShelvesInRange.Remove(colliderShelf);
        }
    }
}
