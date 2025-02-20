using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public UnityEvent ue_sceneReset;

    public int levelsUnlocked;
    public int levelInt;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        DontDestroyOnLoad(this);

        if (PlayerPrefs.HasKey("LevelsUnlocked"))
            levelsUnlocked = PlayerPrefs.GetInt("LevelsUnlocked");
    }


    public void LoadLevel(int level)
    {
        SceneManager.LoadScene("Scenes/Levels/Level" + level);
    }

    public void ToHomeMenu()
    {
        Debug.Log("Hello from LevelManager");
        SceneManager.LoadScene("Scenes/MainMenu");
    }

    public void PauseLevel()
    {
        Time.timeScale = 0;
    }

    public void ResumeLevel()
    {
        Time.timeScale = 1;
    }

    public void RestartLevel()
    {
        ue_sceneReset.Invoke();
    }
}
