using UnityEngine;

public class LevelButton : MonoBehaviour
{
    public int level;
    public void RequestToLoadLevel()
    {
        transform.parent.GetComponent<LevelButtonManager>().DisableAllButtons();
        LevelManager.instance.LoadLevel(level);
    }
}
