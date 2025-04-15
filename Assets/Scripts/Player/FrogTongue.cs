using UnityEngine;

// Rope swinging/grappling hook courtesy of "Dave/GameDevelopment": https://youtu.be/HPjuTK91MA8?si=USOfgZWOexhI0f0U
// No github was provided so video link will have to do
public class FrogTongue : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject frog;
    public GameObject mouth;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private LineRenderer _lineRenderer;

    [Range(1, 100)][SerializeField] private int _speedMult;
    public float maxRange;
    [SerializeField] private float _distanceFromPlayer;
    [SerializeField] private bool _isRetracting;
    [SerializeField] private Gradient _outGrad, _swingingGrad;

    private Vector3 swingPoint;
    private SpringJoint _springJoint;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        GameManager.instance.ue_sceneReset.AddListener(OnReset);
        _springJoint = frog.GetComponent<SpringJoint>();
    }

    public void OnReset()
    {
        playerController.canAttack = true;
        transform.localPosition = Vector3.zero;
        StopSwinging();
        _isRetracting = false;
        gameObject.SetActive(false);
    }

    public void SetTarget(Vector3 target)
    {
        _rigidbody.linearVelocity = -(transform.position - target).normalized * _speedMult;
        _lineRenderer.colorGradient = _outGrad;
    }

    void FixedUpdate()
    {
        Vector3 toPlayer = transform.position - mouth.transform.position;
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
            StartSwinging();
    }

    void StartSwinging()
    {
        playerController.canJump = false;
        playerController.swinging = true;
        _rigidbody.linearVelocity = Vector3.zero;

        _springJoint.connectedAnchor = transform.position;

        float swingDistance = (frog.transform.position - transform.position).magnitude;

        _springJoint.maxDistance = swingDistance * 0.95f;
        _springJoint.minDistance = swingDistance * 0.25f;
        _springJoint.massScale = 1;

        _lineRenderer.colorGradient = _swingingGrad;
    }

    // bool returns true if we were swinging
    public bool StopSwinging()
    {
        if (playerController.isGrounded) playerController.canJump = true;
        if (gameObject.activeSelf) _isRetracting = true;
        _lineRenderer.colorGradient = _outGrad;
        if (playerController.swinging)
        {
            playerController.swinging = false;
            _springJoint.massScale = 0;
            return true;
        }
        else
            return false;

    }

    void LateUpdate()
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, mouth.transform.position);
    }
}
