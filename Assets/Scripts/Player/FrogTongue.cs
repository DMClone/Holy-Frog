using UnityEngine;

// Rope swinging/grappling hook courtesy of "Dave/GameDevelopment": https://youtu.be/HPjuTK91MA8?si=USOfgZWOexhI0f0U
public class FrogTongue : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private GameObject _frog;
    [SerializeField] private GameObject _mouth;
    [SerializeField] private SpringJoint _springJoint;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private LineRenderer _lineRenderer;

    [Header("Settings")]
    [Range(1, 100)][SerializeField] private int _speedMult;
    public float maxRange;
    [SerializeField] private float _distanceFromPlayer;
    [SerializeField] private bool _isRetracting;
    [SerializeField] private Gradient _outGrad, _swingingGrad;

    private Vector3 swingPoint;

    private void Awake()
    {
        GameManager.instance.ue_sceneReset.AddListener(OnReset);
    }

    public void OnReset()
    {
        _playerController.canAttack = true;
        StopSwinging();
        _isRetracting = false;
        gameObject.SetActive(false);
    }

    public void SetTarget(Vector3 target)
    {
        transform.position = _mouth.transform.position;
        _rigidbody.linearVelocity = -(transform.position - target).normalized * _speedMult;
        _lineRenderer.colorGradient = _outGrad;
    }

    void FixedUpdate()
    {
        Vector3 toPlayer = transform.position - _mouth.transform.position;
        _distanceFromPlayer = toPlayer.magnitude;

        if (_isRetracting && _distanceFromPlayer <= 1)
        {
            OnReset();
            return;
        }

        if (!_playerController.isSwinging && _distanceFromPlayer >= maxRange)
            _isRetracting = true;

        if (_isRetracting)
            _rigidbody.linearVelocity = -toPlayer.normalized * _speedMult;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
            StartSwinging();
    }

    void StartSwinging()
    {
        _playerController.canJump = false;
        _playerController.isSwinging = true;
        _rigidbody.linearVelocity = Vector3.zero;

        _springJoint.connectedAnchor = transform.position;

        float swingDistance = (_frog.transform.position - transform.position).magnitude;

        _springJoint.maxDistance = swingDistance * 0.95f;
        _springJoint.minDistance = swingDistance * 0.25f;
        _springJoint.massScale = 1;

        _lineRenderer.colorGradient = _swingingGrad;
    }

    // bool returns true if we were swinging
    public bool StopSwinging()
    {
        if (_playerController.isGrounded) _playerController.canJump = true;
        if (gameObject.activeSelf) _isRetracting = true;
        _lineRenderer.colorGradient = _outGrad;
        if (_playerController.isSwinging)
        {
            _playerController.isSwinging = false;
            _springJoint.massScale = 0;
            return true;
        }
        else
            return false;

    }

    void LateUpdate()
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, _mouth.transform.position);
    }
}
