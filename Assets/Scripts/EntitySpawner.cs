using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    public GameObject _gameObjectToSpawn;
    private GameObject _child;
    private void Awake()
    {
        GameManager.instance.ue_sceneReset.AddListener(OnReset);
        OnReset();
    }

    private void OnReset()
    {
        if (_child != null)
            Destroy(_child);
        _child = Instantiate(_gameObjectToSpawn, transform.position, transform.rotation);
    }

    void OnDrawGizmosSelected()
    {
        Debug.DrawLine(transform.position, (transform.position + transform.forward * 5), Color.white);
    }
}