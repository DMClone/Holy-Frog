using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    public GameObject spawningObject;
    private ISpawnable _spawnable;

    private void Awake()
    {
        var scripts = spawningObject.GetComponents<MonoBehaviour>();

        foreach (var monoBehavior in scripts)
        {
            var tempTest = monoBehavior as ISpawnable;

            if (tempTest != null)
            {
                _spawnable = tempTest;
                break;
            }
        }

        GameManager.instance.ue_sceneReset.AddListener(OnReset);
        OnReset();
    }

    private void OnReset()
    {
        _spawnable.OnReset();
    }

    void OnDrawGizmosSelected()
    {
        Debug.DrawLine(transform.position, (transform.position + transform.forward * 5), Color.white);
    }
}