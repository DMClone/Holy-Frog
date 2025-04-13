using UnityEngine;

public class EnemyBehavior : MonoBehaviour, ISpawnable
{
    protected GameManager _gameManager;
    protected Transform _playerTrans;
    protected Health _health;
    protected Vector3 _startPos;

    protected virtual void Awake()
    {
        _gameManager = GameManager.instance;
        _health = GetComponent<Health>();
        _playerTrans = PlayerController.instance.transform;
        _startPos = transform.position;
        Debug.Log(_startPos);

        GameManager.instance.ue_sceneReset.AddListener(OnReset);
    }

    public virtual void OnReset()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        transform.position = _startPos;
        _health.OnReset();
        Debug.Log("Did EB");
    }

    protected void TouchedPlayer()
    {
        _gameManager.RestartLevel();
    }
}
