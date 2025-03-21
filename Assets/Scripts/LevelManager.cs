using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public UnityEvent uesceneReset;

    [SerializeField] private GameObject canvas;
    [SerializeField] private PlayerController _playerController;

    public int levelsUnlocked;
    [SerializeField] private int _maxLevels;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        // PlayerPrefs.DeleteKey("LevelsUnlocked");

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
        string path = "Scenes/Levels/Level" + level;
        StartCoroutine(SceneLoad(path));
    }

    private IEnumerator SceneLoad(string scenePath)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scenePath);

        while (!asyncLoad.isDone)
        {
            yield return null;
            _playerController.enabled = true;
        }
    }

    public void ToHome()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1;
        _playerController.enabled = false;
        string path = "Scenes/MainMenu";
        StartCoroutine(SceneLoad(path));
    }

    public void UpdateUnlock(int levelUnlocked)
    {
        if (levelsUnlocked < levelUnlocked && levelUnlocked <= _maxLevels)
        {
            PlayerPrefs.SetInt("LevelsUnlocked", levelUnlocked);
            levelsUnlocked = levelUnlocked;
        }
    }
}
