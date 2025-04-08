using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    public GameObject child;
    [SerializeField] public ISpawnable spawnable;
    private void Awake()
    {
        ISpawnable[] scripts = child.GetComponents<ISpawnable>();
        Debug.Log(scripts.Length);
        GameManager.instance.ue_sceneReset.AddListener(OnReset);
        OnReset();
    }

    private void OnReset()
    {
        spawnable.OnReset();
    }

    void OnDrawGizmosSelected()
    {
        Debug.DrawLine(transform.position, (transform.position + transform.forward * 5), Color.white);
    }
}