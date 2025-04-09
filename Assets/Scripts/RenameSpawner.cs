using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(EntitySpawner))]
public class RenameSpawner : MonoBehaviour
{
    private EntitySpawner _entitySpawner;

    private void Awake()
    {
        _entitySpawner = GetComponent<EntitySpawner>();
    }

    void OnEnable()
    {
        if (_entitySpawner.spawningObject != false)
            gameObject.name = _entitySpawner.spawningObject.name + " Spawner";
        else
            gameObject.name = "Empty Spawner";
    }
}