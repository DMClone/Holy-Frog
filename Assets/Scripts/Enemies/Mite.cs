using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Mite : EnemyBehavior
{
    protected enum MiteAnimationState
    {
        Idle,
        Run,
        Attack,
        Death
    }

    [Header("Dependencies")]
    [SerializeField] protected Animator _animator;
    [SerializeField][ReadOnly] protected MiteAnimationState _animationState;
    protected NavMeshAgent _navMeshAgent;
    protected Vector3 _lastPlayerPos;

    protected override void Start()
    {
        base.Start();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public override void OnReset()
    {
        _navMeshAgent.enabled = false;
        base.OnReset();
        _navMeshAgent.velocity = Vector3.zero;
        _navMeshAgent.enabled = true;
        _lastPlayerPos = Vector3.positiveInfinity;
    }

    protected virtual void FixedUpdate()
    {
        if (Vector3.Distance(_lastPlayerPos, _playerTransform.position) > 0.5f)
            ToPlayer();
    }

    protected float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, _playerTransform.position);
    }

    protected virtual void ToPlayer()
    {
        NavMeshPath newPath = new NavMeshPath();
        _navMeshAgent.CalculatePath(_playerTransform.position, newPath);
        if (newPath.status == NavMeshPathStatus.PathComplete)
        {
            _navMeshAgent.SetPath(newPath);
            _lastPlayerPos = _playerTransform.position;
            if (_animationState != MiteAnimationState.Run)
            {
                _animator.SetTrigger("Run");
                _animationState = MiteAnimationState.Run;
            }
        }
        else
        {
            _navMeshAgent.ResetPath();
            _navMeshAgent.velocity = Vector3.zero;
            if (_animationState != MiteAnimationState.Idle)
            {
                _animator.SetTrigger("Idle");
                _animationState = MiteAnimationState.Idle;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            TouchedPlayer();
        }
    }
}
