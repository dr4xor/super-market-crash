using UnityEngine;
using System.Collections;

public class PlayerMeshBlinker : MonoBehaviour
{
    [SerializeField] private float blinkDuration = 1.5f;
    [SerializeField] private float blinkInterval = 0.2f;

    private Renderer[] _renderers;
    private bool _isBlinking = false;
    private void Start()
    {
        _renderers = GetComponentsInChildren<Renderer>();
    }

    public void StartBlinking()
    {
// Check if already blinking
        if (_isBlinking)
        {
            return;
        }


        _renderers = GetComponentsInChildren<Renderer>();
        _isBlinking = true;
        StartCoroutine(Blink());
    }

    private IEnumerator Blink()
    {
        float elapsed = 0f;
        while (elapsed < blinkDuration)
        {
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
            foreach (var renderer in _renderers)
            {
                renderer.enabled = !renderer.enabled;
            }
        }
        foreach (var renderer in _renderers)
        {
            renderer.enabled = true;
        }
        _isBlinking = false;
        
        yield return null;

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
