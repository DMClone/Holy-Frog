using UnityEngine;
using UnityEngine.SceneManagement;

public class InLevelButton : MonoBehaviour
{
    [SerializeField] private int _nextLevel;

    public void RequestToLoadLevel()
    {
        LevelManager.instance.LoadLevel(_nextLevel);
    }

    public void RequestToLoadHome()
    {
        LevelManager.instance.ToHome();
    }

    public void PauseToggle()
    {
        GameManager.instance.PauseToggle(PauseSetting.Resume, false);
    }

    public void RestartLevel()
    {
        GameManager.instance.RestartLevel();
    }
}
