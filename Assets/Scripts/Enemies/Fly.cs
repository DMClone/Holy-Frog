using UnityEngine;

public class Fly : MonoBehaviour
{
    private Transform _playerPos;
    private bool _playerFound;

    private void Awake()
    {
        _playerPos = PlayerController.instance.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
            _playerFound = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
            _playerFound = false;
    }

    void FixedUpdate()
    {
        if (_playerFound)
            transform.position += (_playerPos.position - transform.position).normalized * 0.05f;
    }
}
