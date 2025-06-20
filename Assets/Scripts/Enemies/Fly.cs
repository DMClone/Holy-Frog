using UnityEngine;

public class Fly : EnemyBehavior
{
    private bool _playerFound;
    [SerializeField][Range(0, 2)] private float _touchRadius;
    [SerializeField][Range(0, 1)] private float _speed;
    [SerializeField][Range(0, 1)] private float _snapDistance;
    private Vector3 toPlayer;

    private void Update()
    {
        if (_playerFound)
        {
            Quaternion targetRotation = Quaternion.LookRotation(-toPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);
        }
        else
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.position - _startPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);
        }
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

    private void FixedUpdate()
    {
        toPlayer = (_playerTransform.position - transform.position).normalized;
        if (_playerFound && Physics.Raycast(transform.position, toPlayer) == _playerTransform.gameObject)
            transform.position += toPlayer * _speed;
        else if (Vector3.Distance(transform.localPosition, _startPos) > _snapDistance)
            transform.position += (_startPos - transform.position).normalized * _speed;
        else
            transform.position = _startPos;

        if (Vector3.Distance(transform.position, _playerTransform.position) < _touchRadius)
            TouchedPlayer();
    }
}
