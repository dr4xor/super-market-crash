using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private float runningSpeedThreshold = 1f;
    [SerializeField] private string isRunningParamName = "isRunning";
    [SerializeField] private AnimationCurve velocityToAnimSpeedCurve;
    [Space]
    [SerializeField] private string grabItemParam = "isGrabbing";

    private Animator _animator;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        fetchNewAnimatorIfDestroyed();

        float speed = _rigidbody.linearVelocity.magnitude;

        float animSpeed = velocityToAnimSpeedCurve.Evaluate(speed);

        _animator.speed = animSpeed;

        if (speed > runningSpeedThreshold)
        {
            _animator.SetBool(isRunningParamName, true);
        }
        else
        {
            _animator.SetBool(isRunningParamName, false);
        }
    }

    public void GrabItem()
    {
Debug.Log("GrabItem");

        _animator.SetTrigger(grabItemParam);
    }

    private void fetchNewAnimatorIfDestroyed()
    {
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }
    }
}
