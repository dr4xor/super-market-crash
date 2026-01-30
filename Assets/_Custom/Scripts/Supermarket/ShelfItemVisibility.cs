using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Supermarket
{
    public class ShelfItemVisibility : MonoBehaviour
    {
        [SerializeField] private Image image;
        
        private void Start()
        {
            image.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerInputReceiver>(out var playerInputReceiver))
            {
                image.gameObject.SetActive(true);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<PlayerInputReceiver>(out var playerInputReceiver))
            {
                image.gameObject.SetActive(false);
            }
        }
    }
}
