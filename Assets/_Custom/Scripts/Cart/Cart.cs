using System;
using JetBrains.Annotations;
using SappUnityUtils.Numbers;
using UnityEngine;

public class Cart : MonoBehaviour
{
    [SerializeField] private float fixedYPosition = 0.1f;
    [SerializeField] private float forceFactor = 10f;
    [SerializeField] private float forceFactorBrake = 10f;
    [SerializeField] private Transform toRotate;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float maxSpeed = 6f;
    //[SerializeField] private float acceleration = 8f;
    [SerializeField] private AnimationCurve dashMoveCurve;
    [SerializeField] private float dashTime = 0.5f;
    [SerializeField] private float forceDashing = 2f;
    [SerializeField] private float forceFactorBrakeDashing = 2f;
    [SerializeField] private float maxSpeedDashing = 2f;
    [Space]
    [SerializeField] private bool useButtonMashDash;
    [SerializeField] private float buttonMashMaxTimeBetweenPresses = 0.17f;
    [SerializeField] private int buttonMashMaxAccumulatedPresses = 10;
    [Space]
    [Header("Particles")]
    [SerializeField] private ParticleSystem sprintParticles;
    [SerializeField] private AnimationCurve sprintPartsEmissionByByDashFactor;

    /*
    [Header("Movement")]
    [SerializeField] private float _maxSpeed = 6f;
    [Tooltip("How quickly the cart reaches target speed. Lower = more slidey.")]
    [SerializeField] private float _acceleration = 8f;
    [Tooltip("How quickly velocity direction follows input. Lower = more drift when turning.")]
    [SerializeField] private float _turnResponsiveness = 6f;

    [Header("Rotation (optional)")]
    [Tooltip("If > 0, cart model rotates toward movement direction.")]
    [SerializeField] private float _rotationFollowSpeed = 5f;
    */


    /// Button mash dash
    private int _accumulatedMashPresses = 0;
    private float _timeSinceLastMashPress = 0f;
    private bool _hasButtonMashBeenPressed = false;

    private Vector3 _moveInput;
    private Rigidbody _rigidbody;
    private float _curYRotation;

    private float _dashS = 0f;
    private bool _isDashing = false;

    private CartShaker _cartShaker;
    public CartShaker CartShaker => _cartShaker;
    private PlayerAnimationController _playerAnimationController;
    public PlayerAnimationController PlayerAnimationController => _playerAnimationController;

    private CartItemsContainer _cartItemsContainer;
    public CartItemsContainer CartItemsContainer => _cartItemsContainer;

    private MovingAverage<float> _movingAverageVelocity = new MovingAverage<float>(64);
    public MovingAverage<float> MovingAverageVelocity => _movingAverageVelocity;


    private float _curDashFactor = 0f;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = Vector3.zero;
        _rigidbody.inertiaTensorRotation = Quaternion.identity;

        _cartShaker = GetComponentInChildren<CartShaker>();

        // Recommended for drift: low drag so our code controls the feel
        //if (_rigidbody.linearDamping < 0.5f)
        //    _rigidbody.linearDamping = 0.5f;

        _playerAnimationController = GetComponent<PlayerAnimationController>();
        _cartItemsContainer = GetComponentInChildren<CartItemsContainer>();
    }

    private void FixedUpdate()
    {
        float dashFactor = handleDash();


        _curDashFactor = Mathf.MoveTowards(_curDashFactor, dashFactor, Time.fixedDeltaTime * 4f);

        float curMaxSpeed = Mathf.Lerp(maxSpeed, maxSpeedDashing, dashFactor);
        Vector3 desiredVelocity = _moveInput * curMaxSpeed;

        Vector3 forceDelta = desiredVelocity - _rigidbody.linearVelocity;

        float angleBetweenDeltaAndVelocity = Vector3.Angle(forceDelta, _rigidbody.linearVelocity);

        float usedForceFactor = Mathf.Lerp(forceFactor, forceDashing, dashFactor);

        if (angleBetweenDeltaAndVelocity > 90f)
        {
            usedForceFactor = Mathf.Lerp(forceFactorBrake, forceFactorBrakeDashing, dashFactor);
        }

        // Add force to push the rigidboody towards the desired velocity
        _rigidbody.AddForce(forceDelta * usedForceFactor);


        //_rigidbody.AddForce(_moveInput * forceFactor);

        // Limit the velocity to the max speed

        Vector3 currentVelocity = _rigidbody.linearVelocity;
        float currentSpeed = currentVelocity.magnitude;
        if (currentSpeed > curMaxSpeed)
        {
            _rigidbody.linearVelocity = currentVelocity.normalized * curMaxSpeed;
        }

        if (_moveInput.magnitude > 0.01f)
        {
            float targetRotaion = Mathf.Atan2(_moveInput.x, _moveInput.z) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.Euler(0f, targetRotaion, 0f);

            toRotate.rotation = Quaternion.RotateTowards(toRotate.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        //Debug.Log("Current velocity: " + _rigidbody.linearVelocity.magnitude);

        _movingAverageVelocity.AddValue(_rigidbody.linearVelocity.magnitude);

        // Force clamp the rotation of the rigidbody to be flat
        //_rigidbody.rotation = Quaternion.Euler(0f, _rigidbody.rotation.eulerAngles.y, 0f);

        //_rigidbody.position = new Vector3(_rigidbody.position.x, fixedYPosition, _rigidbody.position.z);

        handleButtonMashDash();

        updateParticleSystemEmission();
    }

    private float handleDash()
    {
        if (useButtonMashDash)
        {
            return _accumulatedMashPresses / ((float)buttonMashMaxAccumulatedPresses);
        }

        if (!_isDashing)
        {
            return 0f;
        }

        _dashS += Time.fixedDeltaTime / dashTime;
        float dashCurveValue = dashMoveCurve.Evaluate(_dashS);


        return dashCurveValue;
    }

    public void ProvideMoveDirection(Vector2 moveDirection)
    {
        _moveInput = new Vector3(moveDirection.x, 0f, moveDirection.y);
    }

    public void PerformDash()
    {
        if (useButtonMashDash)
        {
            _hasButtonMashBeenPressed = true;
        }
        else
        {
            _isDashing = true;
            _dashS = 0f;
        }
    }

    private void handleButtonMashDash()
    {
        if (!useButtonMashDash)
        {
            return;
        }

        _timeSinceLastMashPress += Time.fixedDeltaTime;
        if (_timeSinceLastMashPress > buttonMashMaxTimeBetweenPresses)
        {
            if (_hasButtonMashBeenPressed)
            {
                _accumulatedMashPresses++;
                _hasButtonMashBeenPressed = false;
            }
            else
            {
                _accumulatedMashPresses -= 3;
            }

            _timeSinceLastMashPress -= buttonMashMaxTimeBetweenPresses;

            _accumulatedMashPresses = Mathf.Clamp(_accumulatedMashPresses, 0, buttonMashMaxAccumulatedPresses);
        }
    }


    public void ResetCurrentDashFactor()
    {
        _accumulatedMashPresses = 0;
    }

    private void updateParticleSystemEmission()
    {
        float emissionFactor = sprintPartsEmissionByByDashFactor.Evaluate(_curDashFactor);
        ParticleSystem.EmissionModule emission = sprintParticles.emission;
        emission.rateOverDistance = emissionFactor;
    }

    /*
        private void performDriftMovement()
        {
            // Map stick input to world XZ (Y = up). Magnitude = how much throttle.
            Vector3 inputWorld = new Vector3(_moveInput.x, 0f, _moveInput.y);
            float inputMagnitude = inputWorld.magnitude;
            if (inputMagnitude > 1f)
                inputMagnitude = 1f;

            Vector3 desiredDirection = inputMagnitude > 0.01f ? inputWorld.normalized : Vector3.zero;
            float desiredSpeed = inputMagnitude * _maxSpeed;
            Vector3 desiredVelocity = desiredDirection * desiredSpeed;

            Vector3 currentVelocity = _rigidbody.linearVelocity;
            currentVelocity.y = 0f; // keep vertical velocity (gravity/jumps) untouched

            // Accelerate magnitude: reach target speed over time
            float currentSpeed = currentVelocity.magnitude;
            float newSpeed = Mathf.MoveTowards(currentSpeed, desiredSpeed, _acceleration * Time.fixedDeltaTime);

            // Turn toward desired direction with lag (this creates the drift)
            Vector3 newDirection;
            if (newSpeed < 0.01f || desiredSpeed < 0.01f)
            {
                newDirection = desiredDirection.sqrMagnitude > 0.01f ? desiredDirection : (currentSpeed > 0.01f ? currentVelocity.normalized : desiredDirection);
            }
            else if (currentSpeed < 0.01f)
            {
                newDirection = desiredDirection; // from standstill, go straight toward input
            }
            else
            {
                Vector3 currentDirection = currentVelocity.normalized;
                newDirection = Vector3.Slerp(currentDirection, desiredDirection, _turnResponsiveness * Time.fixedDeltaTime).normalized;
            }

            Vector3 newVelocity = newDirection * newSpeed;
            newVelocity.y = _rigidbody.linearVelocity.y;
            _rigidbody.linearVelocity = newVelocity;

            // Optional: rotate cart to face movement for extra juice
            if (_rotationFollowSpeed > 0f && newVelocity.sqrMagnitude > 0.01f)
            {
                Vector3 flatVel = newVelocity;
                flatVel.y = 0f;
                Quaternion targetRotation = Quaternion.LookRotation(flatVel, Vector3.up);
                _rigidbody.rotation = Quaternion.Slerp(_rigidbody.rotation, targetRotation, _rotationFollowSpeed * Time.fixedDeltaTime);
            }
        }
    */

}
