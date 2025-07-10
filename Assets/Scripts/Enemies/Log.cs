using UnityEngine;

public class Log : MonoBehaviour
{
    [Header("Dependencies")]
    public Rigidbody rb;
    private GameManager _gameManager;
    [Header("Settings")]
    [SerializeField] private Vector3 _startScale;
    [SerializeField] private float _minScale = 0.1f;
    [Tooltip("Time to reach min scale in seconds")][SerializeField] private float _scaleSpeed = 4f;
    [SerializeField] private float _bounceHeight = 5f;
    [ReadOnly] public bool thrown;
    private Vector3 _startPos;
    private Vector3 _lastVelocity;

    private void Awake()
    {
        transform.localScale = _startScale;
    }

    void Start()
    {
        _gameManager = GameManager.instance;
        _gameManager.ue_sceneReset.AddListener(OnReset);
    }

    void OnDisable()
    {
        transform.localScale = new Vector3(_startScale.x, _startScale.y, _startScale.z);
        thrown = false;
        Debug.Log("Disabled");
    }

    public void OnReset()
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);

        rb.isKinematic = true;
        transform.localScale = _startScale;
    }

    private void FixedUpdate()
    {
        if (!thrown) return;
        _lastVelocity = rb.linearVelocity;

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, _scaleSpeed * Time.fixedDeltaTime);

        if (transform.localScale.x < _minScale)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.linearVelocity = new Vector3(_lastVelocity.x, _bounceHeight, _lastVelocity.z);

        if (collision.gameObject.GetComponent<PlayerController>())
        {
            TouchedPlayer();
        }
    }

    private void TouchedPlayer()
    {
        _gameManager.RestartLevel();
    }
}
