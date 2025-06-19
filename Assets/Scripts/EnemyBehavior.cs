using UnityEngine;

public class EnemyBehavior : MonoBehaviour, ISpawnable
{
    protected GameManager _gameManager;
    protected Transform _playerTransform;
    protected Health _health;
    protected Vector3 _startPos;

    protected virtual void Start()
    {
        _gameManager = GameManager.instance;
        _health = GetComponent<Health>();
        _playerTransform = PlayerController.instance.transform;
        _startPos = transform.position;

        GameManager.instance.ue_sceneReset.AddListener(OnReset);
    }

    public virtual void OnReset()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        transform.position = _startPos;
        _health.OnReset();
    }

    protected void TouchedPlayer()
    {
        _gameManager.RestartLevel();
    }
}
