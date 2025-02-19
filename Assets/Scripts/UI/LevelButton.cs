using UnityEngine;

public class LevelButton : MonoBehaviour
{
    public int level;
    public void RequestToLoadLevel()
    {
        LevelManager.instance.LoadLevel(level);
    }
}
