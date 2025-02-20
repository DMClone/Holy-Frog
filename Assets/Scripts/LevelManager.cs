using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public UnityEvent ue_sceneReset;

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
        canvas = CanvasInstance.instance.gameObject;
    }

    public void LoadLevel(int level)
    {
        SceneManager.LoadScene("Scenes/Levels/Level" + level);
    }

    public void ToHome()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }

    public void PauseLevel()
    {
        Time.timeScale = 0;
    }

    public void UnpauseLevel()
    {
        Time.timeScale = 1;
        canvas.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void RestartLevel()
    {
        ue_sceneReset.Invoke();
    }
}
