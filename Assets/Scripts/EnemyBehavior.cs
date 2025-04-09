using UnityEngine;

public class EnemyBehavior : MonoBehaviour, ISpawnable
{
    protected Health _health;

    protected virtual void Awake()
    {
        _health = GetComponent<Health>();
    }

    public virtual void OnReset()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        _health.OnReset();
    }
}
