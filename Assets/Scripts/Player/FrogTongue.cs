using UnityEngine;

public class FrogTongue : MonoBehaviour
{
    public GameObject frog;
    [SerializeField] private Rigidbody _rigidbody;

    [Range(1, 25)][SerializeField] private int _speedMult;
    [SerializeField] private float _maxRange;
    [SerializeField] private float _distanceFromPlayer;
    [SerializeField] private bool _isRetracting;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void OnReset()
    {

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
            PlayerController.instance.canAttack = true;
            transform.localPosition = Vector3.zero;
            _isRetracting = false;
            gameObject.SetActive(false);
            return;
        }

        if (_distanceFromPlayer >= _maxRange)
            _isRetracting = true;

        if (_isRetracting)
            _rigidbody.linearVelocity = -toPlayer.normalized * _speedMult;
    }
}
