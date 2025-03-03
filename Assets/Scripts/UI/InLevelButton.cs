using UnityEngine;
using UnityEngine.SceneManagement;

public class InLevelButton : MonoBehaviour
{
    [SerializeField] private int _nextLevel;
    private void Awake()
    {
        _nextLevel = GetLevelIndex();
    }

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
        GameManager.instance.PauseToggle();
    }

    private int GetLevelIndex()
    {
        int levelIndex = SceneManager.GetActiveScene().name.IndexOf("Level");
        string numberPart = SceneManager.GetActiveScene().name.Substring(levelIndex + "Level".Length);

        int.TryParse(numberPart, out int levelNumber);
        return levelNumber;
    }
}
