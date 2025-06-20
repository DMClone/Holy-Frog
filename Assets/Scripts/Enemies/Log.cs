using UnityEngine;

public class Log : MonoBehaviour
{
    [Header("Dependencies")]
    public Rigidbody _rigidbody;
    private GameManager _gameManager;
    [Header("Settings")]
    [SerializeField] private float _minScale = 0.1f;
    [Tooltip("Time to reach min scale in seconds")][SerializeField] private float _scaleSpeed = 4f;
    private Vector3 _startPos;
    private Vector3 _lastVelocity;

    void Start()
    {
        _gameManager = GameManager.instance;
        _gameManager.ue_sceneReset.AddListener(OnReset);
        _startPos = transform.position;
    }

    public void OnReset()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        transform.position = new Vector3(0, 0, 0);
        transform.localScale = Vector3.one;
        _rigidbody.linearVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        _lastVelocity = _rigidbody.linearVelocity;

        if (transform.localScale.x > 0.1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, _scaleSpeed * Time.fixedDeltaTime);
            if (transform.localScale.x < _minScale)
            {
                transform.localScale = new Vector3(_minScale, _minScale, _minScale);
                gameObject.SetActive(false);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        _rigidbody.linearVelocity = _lastVelocity;
        _rigidbody.linearVelocity += Vector3.up * 5f;
    }
}
