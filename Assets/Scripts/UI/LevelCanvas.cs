using UnityEngine;

public class LevelCanvas : MonoBehaviour
{
    public static LevelCanvas instance;
    // [SerializeField] private GameObject

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void TogglePauseScreen()
    {

    }
}
