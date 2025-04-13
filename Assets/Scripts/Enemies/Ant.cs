using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Ant : EnemyBehavior
{
    private NavMeshAgent _navMeshAgent;
    private Vector3 _lastPlayerPos;

    protected override void Awake()
    {
        base.Awake();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public override void OnReset()
    {
        _navMeshAgent.enabled = false;
        base.OnReset();
        _navMeshAgent.velocity = Vector3.zero;
        _navMeshAgent.enabled = true;
        _lastPlayerPos = Vector3.positiveInfinity;
        Debug.Log("Did Ant");
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(_lastPlayerPos, _playerTrans.transform.position) < 0.5f)
            return;

        NavMeshPath newPath = new NavMeshPath();
        _navMeshAgent.CalculatePath(_playerTrans.transform.position, newPath);
        if (newPath.status == NavMeshPathStatus.PathComplete)
        {
            _navMeshAgent.SetPath(newPath);
            _lastPlayerPos = _playerTrans.position;
            Debug.Log("Set a cool path");
        }
        else
        {
            _navMeshAgent.ResetPath();
            _navMeshAgent.velocity = Vector3.zero;
            Debug.Log("Couldn't set a path");

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
