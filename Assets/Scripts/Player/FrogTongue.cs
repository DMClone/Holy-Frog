using UnityEngine;

public class FrogTongue : MonoBehaviour
{
    public GameObject frog;
    [SerializeField] private Rigidbody _rigidbody;

    private float distanceFromPlayer;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void OnDisable()
    {
        transform.position = Vector3.zero;
    }

    void FixedUpdate()
    {
        distanceFromPlayer = (transform.position - frog.transform.position).magnitude;
    }
}
