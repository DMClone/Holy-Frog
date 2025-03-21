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
        if (_entitySpawner._gameObjectToSpawn != false)
            gameObject.name = _entitySpawner._gameObjectToSpawn.name + " Spawner";
        else
            gameObject.name = "Empty Spawner";
    }
}