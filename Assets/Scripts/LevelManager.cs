using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    static LevelManager instance;

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
}
