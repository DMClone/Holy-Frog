using UnityEngine;

public class FrogTongue : MonoBehaviour
{
    public GameObject frog;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private LineRenderer _lineRenderer;

    [Range(1, 25)][SerializeField] private int _speedMult;
    public float maxRange;
    [SerializeField] private float _distanceFromPlayer;
    [SerializeField] private bool _isRetracting;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        GameManager.instance.ue_sceneReset.AddListener(OnReset);
    }

    public void OnReset()
    {
        PlayerController.instance.canAttack = true;
        transform.localPosition = Vector3.zero;
        _isRetracting = false;
        gameObject.SetActive(false);
    }

    public void SetTarget(Vector3 target)
    {
        _rigidbody.linearVelocity = -(transform.position - target).normalized * _speedMult;
    }

    void FixedUpdate()
    {
        Vector3 toPlayer = transform.position - frog.transform.position;
        _distanceFromPlayer = toPlayer.magnitude;

        if (_isRetracting && _distanceFromPlayer <= 1)
        {
            OnReset();
            return;
        }

        if (_distanceFromPlayer >= maxRange)
            _isRetracting = true;

        if (_isRetracting)
            _rigidbody.linearVelocity = -toPlayer.normalized * _speedMult;
    }


    private void Retract()
    {
        _isRetracting = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
            Retract();
    }

    void Update()
    {

        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, frog.transform.position);
    }
}
