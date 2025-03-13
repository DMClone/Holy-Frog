using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public UnityEvent uesceneReset;

    [SerializeField] private GameObject canvas;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += GetCanvas;
    }

    private void GetCanvas(Scene scene, LoadSceneMode mode)
    {
        canvas = LevelCanvas.instance.gameObject;
    }

    public void LoadLevel(int level)
    {
        SceneManager.LoadScene("Scenes/Levels/Level" + level);
    }

    public void ToHome()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1;
        SceneManager.LoadScene("Scenes/MainMenu");
    }
}
