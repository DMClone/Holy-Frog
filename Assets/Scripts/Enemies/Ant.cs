using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Ant : Mite
{
    [Header("Dependencies")]
    [SerializeField] Log[] _logs;
    [Header("Settings")]
    [Tooltip("Distance before needing to stop")][SerializeField][Range(0, 100)] private int _stopDistance;
    [Tooltip("Distance before needing to back up")][SerializeField][Range(0, 100)] private int _backupDistance;
    [SerializeField] private int _throwStacks;
    [SerializeField] private int _maxThrowStacks = 20; // Number of stacks before throwing a log
    [SerializeField] private float _throwInterval = 2f; // Time between holding log and throwing it
    [SerializeField] private float _throwSpeed = 15f;
    [SerializeField] private float _throwHeight = 5f;
    [SerializeField][ReadOnly] private int _currentLog;
    [SerializeField][ReadOnly] private bool _holdingLog;
    public float _distance;

    public override void OnReset()
    {
        base.OnReset();
        _throwStacks = _maxThrowStacks / 2;
    }

    protected override void FixedUpdate()
    {
        float distance = GetDistanceToPlayer();
        _distance = distance;

        if (_backupDistance > distance)
        {
            Debug.Log("1");
            Vector3 fromPlayer = _playerTransform.position - transform.position;
            _navMeshAgent.SetDestination(transform.position - fromPlayer * 2);
            _navMeshAgent.angularSpeed = 0;
            if (_throwStacks != 0) _throwStacks--;
        }

        else if (distance > _stopDistance)
        {
            Debug.Log("2");
            _navMeshAgent.angularSpeed = 360;
            base.FixedUpdate();
            if (!_holdingLog)
                _throwStacks++;

        }
        else if (_navMeshAgent.hasPath)
        {
            Debug.Log("3");
            _navMeshAgent.ResetPath();
            Debug.Log("Stopped");
            if (!_holdingLog)
                _throwStacks++;
        }

        else
            if (!_holdingLog)
            _throwStacks++;

        if (_throwStacks >= _maxThrowStacks)
        {
            StartCoroutine(HoldLog());
        }
    }

    private IEnumerator HoldLog()
    {
        if (_currentLog >= _logs.Length)
        {
            _currentLog = 0;
        }
        Log log = _logs[_currentLog];

        _holdingLog = true;
        log.rb.isKinematic = true;
        log.transform.position = transform.position + Vector3.up * 0.5f;
        log.transform.parent = transform;
        log.gameObject.SetActive(true);
        _throwStacks = 0;
        yield return new WaitForSeconds(_throwInterval);
        log.transform.parent = null;
        ThrowLog(log);
        _currentLog++;
        _holdingLog = false;
    }

    private void ThrowLog(Log log)
    {
        Vector3 toPlayer = (new Vector3(_playerTransform.position.x, transform.position.y, _playerTransform.position.z) - transform.position).normalized;
        log.transform.rotation = Quaternion.LookRotation(toPlayer);
        log.rb.isKinematic = false;
        log.rb.linearVelocity = (toPlayer * _throwSpeed) + Vector3.up * _throwHeight;
        log.thrown = true;
    }
}
