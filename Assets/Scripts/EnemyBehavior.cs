using UnityEngine;

public class EnemyBehavior : MonoBehaviour, ISpawnable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnReset()
    {
        Debug.Log("Enemy reset");
    }
}
