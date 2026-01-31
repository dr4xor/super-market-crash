using DG.Tweening;
using UnityEngine;

public class CartItemsContainer : MonoBehaviour
{

    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeStrength = 0.1f;
    [SerializeField] private int shakeVibrato = 10;
    [SerializeField] private float shakeRandomness = 90f;
    [SerializeField] private bool shakeFadeOut = false;
    [SerializeField] private bool shakeIgnoreTimeScale = true;
    [SerializeField] private float shakeRotationStrength = 10f;
    [SerializeField] private int shakeRotationVibrato = 10;
    [SerializeField] private float shakeRotationRandomness = 90f;
    [SerializeField] private bool shakeRotationFadeOut = false;
    [SerializeField] private bool shakeRotationIgnoreTimeScale = true;
    [SerializeField] private float shakeScaleStrength = 0.1f;
    [SerializeField] private int shakeScaleVibrato = 10;
    [SerializeField] private float shakeScaleRandomness = 90f;
    [SerializeField] private bool shakeScaleFadeOut = false;
    [SerializeField] private bool shakeScaleIgnoreTimeScale = true;

    private float _cooldown;

    public void ShakeDueToCrash()
    {
        if (_cooldown > 0f)
        {
            return;
        }

        transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, shakeFadeOut, shakeIgnoreTimeScale);
        transform.DOShakeRotation(shakeDuration, shakeRotationStrength, shakeRotationVibrato, shakeRotationRandomness, shakeRotationFadeOut);
        transform.DOShakeScale(shakeDuration, shakeScaleStrength, shakeScaleVibrato, shakeScaleRandomness, shakeScaleFadeOut);

        _cooldown = shakeDuration;
    }

    private void Update()
    {
        if (_cooldown <= 0f)
        {
            return;
        }

        _cooldown -= Time.deltaTime;
    }
}
