using UnityEngine;
using UnityEngine.AI;

public class Ant : Mite
{
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
        }

        else if (distance > _stopDistance)
        {
            _navMeshAgent.angularSpeed = 360;
            base.FixedUpdate();

        }
        else if (_navMeshAgent.hasPath)
        {
            _navMeshAgent.ResetPath();
            Debug.Log("Stopped");
        }

    }
}
