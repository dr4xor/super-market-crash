using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    [Header("Character Properties")]
    [SerializeField] private bool isRestocker;
    [Space]
    [SerializeField] private Transform debugTarget;
    [SerializeField] private float targetReachedDistance = 1f;
    [SerializeField] private float minSpeedForRunningAnim = 0.25f;
    [SerializeField] private string animParamNameForRunning = "isRunning";
    [SerializeField] private AnimationCurve animSpeedByVelocity;
    [SerializeField] private float rotationSpeed = 1f;

    private Vector3 _origin;
    private Quaternion _originRotation;
    private Transform _curTarget;
    private string _animParamAtTarget;
    private float _timeToStandingAtTarget;
    private float _timeSpentAtTarget;
    private bool _isParamTrigger;
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private NPCState _currentState;
    public NPCState CurrentState
    {
        get => _currentState;
        set
        {
            if (_currentState == value)
            {
                return;
            }
            _currentState = value;
            OnStateChanged?.Invoke(this, value);
        }
    }

    public delegate void StateChangedEvent(NPCController npc, NPCState newState);
    public event StateChangedEvent OnStateChanged;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _origin = transform.position;
        _originRotation = transform.rotation;

        if (isRestocker)
        {
            RestockingManager.Instance.RegisterRestocker(this);
        }
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case NPCState.AT_ORIGIN:
                _animator.SetBool(animParamNameForRunning, false);
                _animator.speed = 1f;
                rotateTowardsTarget(_originRotation);
                break;
            case NPCState.GOING_TO_TARGET:
                _animator.SetBool(animParamNameForRunning, _navMeshAgent.velocity.magnitude > minSpeedForRunningAnim);
                _animator.speed = animSpeedByVelocity.Evaluate(_navMeshAgent.velocity.magnitude);
                if (_navMeshAgent.remainingDistance <= targetReachedDistance)
                {
                    CurrentState = NPCState.AT_TARGET;
                    playAnimationAtTarget(true);
                    _timeSpentAtTarget = 0f;
                }
                _navMeshAgent.updateRotation = true;
                break;
            case NPCState.AT_TARGET:
                _animator.SetBool(animParamNameForRunning, false);
                _timeSpentAtTarget += Time.deltaTime;
                _animator.speed = 1f;
                if (_timeSpentAtTarget >= _timeToStandingAtTarget)
                {
                    CurrentState = NPCState.GOING_TO_ORIGIN;
                    playAnimationAtTarget(false);
                    _navMeshAgent.SetDestination(_origin);
                }
                rotateTowardsTarget(_curTarget.rotation);
                break;
            case NPCState.GOING_TO_ORIGIN:
                _animator.SetBool(animParamNameForRunning, _navMeshAgent.velocity.magnitude > minSpeedForRunningAnim);
                _animator.speed = animSpeedByVelocity.Evaluate(_navMeshAgent.velocity.magnitude);
                if (_navMeshAgent.remainingDistance <= targetReachedDistance)
                {
                    CurrentState = NPCState.AT_ORIGIN;
                }
                _navMeshAgent.updateRotation = true;
                break;
        }

    }

    private void rotateTowardsTarget(Quaternion targetRotation)
    {
        _navMeshAgent.updateRotation = true;
        // If agent has reached its destination and stopped
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
            {
                // Agent is idle - manually rotate towards target
                _navMeshAgent.updateRotation = false;

                rotateTowards(targetRotation);
            }
        }
    }

    private void rotateTowards(Quaternion targetRotation)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void GoToTarget(Transform target, float timeToSpendAtTarget, string animParamAtTarget = "", bool isParamTrigger = false)
    {
        _curTarget = target;
        CurrentState = NPCState.GOING_TO_TARGET;
        _navMeshAgent.SetDestination(target.position);
        _timeToStandingAtTarget = timeToSpendAtTarget;
        _animParamAtTarget = animParamAtTarget;
        _isParamTrigger = isParamTrigger;
    }

    private void playAnimationAtTarget(bool isAnimationPlaying)
    {
        if (!string.IsNullOrEmpty(_animParamAtTarget))
        {
            if (_isParamTrigger)
            {
                _animator.SetTrigger(_animParamAtTarget);
            }
            else
            {
                _animator.SetBool(_animParamAtTarget, isAnimationPlaying);
            }
        }
    }

    [ContextMenu("Go To Debug Target")]
    public void GoToDebugTarget()
    {
        GoToTarget(debugTarget, 3f);
    }
}

public enum NPCState
{
    AT_ORIGIN,
    GOING_TO_TARGET,
    AT_TARGET,
    GOING_TO_ORIGIN,
}