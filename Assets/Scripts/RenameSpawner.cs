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
        if (_entitySpawner.child != false)
            gameObject.name = _entitySpawner.child.name + " Spawner";
        else
            gameObject.name = "Empty Spawner";
    }
}