using UnityEngine;

public class CanvasInstance : MonoBehaviour
{
    public static CanvasInstance instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
}
