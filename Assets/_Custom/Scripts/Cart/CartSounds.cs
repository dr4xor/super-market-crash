using UnityEngine;

public class CartSounds : MonoBehaviour
{
    [SerializeField] private AudioSource audioSourceCart;
    [SerializeField] private AudioSource audioSourceFootSteps;
    [SerializeField] private AnimationCurve footStepsPerSecondBySpeed;
    [SerializeField] private AudioClip[] footStepsClips;

    private float _playNextFootStepInSeconds = 0f;

    private Cart _cart;
    private void Start()
    {
        _cart = GetComponentInParent<Cart>();
    }

    private void Update()
    {
        float avgSpeed = _cart.MovingAverageVelocity.AverageValue(0.1f);

        handleFootSteps(avgSpeed);

        audioSourceCart.volume = Mathf.Clamp01(avgSpeed * 0.5f);
    }

    private void handleFootSteps(float avgSpeed)
    {
        if (avgSpeed < 0.2f)
        {
            return;
        }

        _playNextFootStepInSeconds -= Time.deltaTime;

        if (_playNextFootStepInSeconds <= 0f)
        {
            audioSourceFootSteps.clip = footStepsClips[Random.Range(0, footStepsClips.Length)];
            audioSourceFootSteps.Play();
            _playNextFootStepInSeconds = 1f / footStepsPerSecondBySpeed.Evaluate(avgSpeed);
        }

    }
}
