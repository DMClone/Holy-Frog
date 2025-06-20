using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Ant : Mite
{
    [Header("Settings")]
    [SerializeField] Log[] _logs;
    [SerializeField] private int _throwStacks;
    [SerializeField] private int _maxThrowStacks = 20; // Number of stacks before throwing a log
    [SerializeField] private float _throwInterval = 2f; // Time between holding log and throwing it
    [SerializeField][ReadOnly] private int _currentLog;
    [SerializeField][ReadOnly] private bool _holdingLog;
    [Tooltip("Distance before needing to stop")][SerializeField][Range(0, 50)] private int _stopDistance;
    [Tooltip("Distance before needing to back up")][SerializeField][Range(0, 50)] private int _backupDistance;
    public float _distance;

    protected override void FixedUpdate()
    {
        float distance = GetDistanceToPlayer();
        _distance = distance;

        if (_backupDistance > distance)
        {
            Vector3 fromPlayer = _playerTransform.position - transform.position;
            _navMeshAgent.SetDestination(transform.position - fromPlayer * 2);
            _navMeshAgent.angularSpeed = 0;
            _throwStacks = 0;
        }

        else if (distance > _stopDistance)
        {
            _navMeshAgent.angularSpeed = 360;
            base.FixedUpdate();
            _throwStacks++;

        }
        else if (_navMeshAgent.hasPath)
        {
            _navMeshAgent.ResetPath();
            Debug.Log("Stopped");
            _throwStacks++;
        }

        if (_throwStacks < _maxThrowStacks)
        {
            StartCoroutine(HoldLog());
        }
    }

    private IEnumerator HoldLog()
    {
        _holdingLog = true;
        yield return new WaitForSeconds(_throwInterval);
        ThrowLog();
        _holdingLog = false;
    }

    private void ThrowLog()
    {
        if (_currentLog >= _logs.Length)
        {
            _currentLog = 0;
        }

        Log log = _logs[_currentLog];
        log.gameObject.SetActive(true);
        log.transform.position = transform.position + Vector3.up * 0.5f; // Adjust height if needed
        log.transform.rotation = Quaternion.LookRotation(_playerTransform.position - transform.position);
        log._rigidbody.linearVelocity = (log.transform.forward * 10f) + Vector3.up * 5f; // Adjust speed and angle as needed

        _currentLog++;
    }
}
