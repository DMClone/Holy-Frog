using UnityEngine;

public class Log : MonoBehaviour
{
    private GameManager _gameManager;
    [SerializeField] private Rigidbody _rigidbody;

    void Start()
    {
        _gameManager = GameManager.instance;
        _gameManager.ue_sceneReset.AddListener(OnReset);
    }

    public void OnReset()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        transform.position = new Vector3(0, 0, 0); // Reset position to origin or any desired position

    }
}
