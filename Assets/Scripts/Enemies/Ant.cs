using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Ant : EnemyBehavior
{
    private PlayerController _playerController;
    private GameManager _gameManager;
    private NavMeshAgent _navMeshAgent;
    private Vector3 _lastPlayerPos;

    protected override void Awake()
    {
        base.Awake();
        _playerController = PlayerController.instance;
        _gameManager = GameManager.instance;
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _gameManager.ue_sceneReset.AddListener(OnReset);
    }

    public override void OnReset()
    {
        base.OnReset();
        _navMeshAgent.velocity = Vector3.zero;
        transform.localPosition = Vector3.zero;
        _navMeshAgent.ResetPath();
        _lastPlayerPos = Vector3.positiveInfinity;
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(_lastPlayerPos, _playerController.transform.position) < 0.5f)
            return;

        NavMeshPath newPath = new NavMeshPath();
        _navMeshAgent.CalculatePath(_playerController.transform.position, newPath);
        if (newPath.status == NavMeshPathStatus.PathComplete)
        {
            _navMeshAgent.SetPath(newPath);
            _lastPlayerPos = _playerController.transform.position;
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
            _gameManager.RestartLevel();
        }
    }
}
