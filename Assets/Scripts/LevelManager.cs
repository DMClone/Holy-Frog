using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Collections;
using Unity.VisualScripting;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public UnityEvent uesceneReset;

    [SerializeField] private LoadingScreen _loadingScreen;
    [SerializeField] private GameObject _canvas;
    private PlayerController _playerController;

    public int levelsUnlocked;
    [SerializeField] private int _maxLevels;

    [SerializeField] private int _levelTransitionDelay;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        // PlayerPrefs.DeleteKey("LevelsUnlocked");

        DontDestroyOnLoad(gameObject);

        if (PlayerPrefs.HasKey("LevelsUnlocked"))
            levelsUnlocked = PlayerPrefs.GetInt("LevelsUnlocked");

    }

    private void Update()
    {
        Debug.Log(Time.timeScale);
    }

    private void Start()
    {
        _loadingScreen = LoadingScreen.instance;
        GetCanvas();
    }

    private void GetCanvas()
    {
        _canvas = LevelCanvas.instance.gameObject;
    }

    public void LoadLevel(int level)
    {
        string path = "Scenes/Levels/Level" + level;
        StartCoroutine(SceneLoad(path, false));
    }

    private IEnumerator SceneLoad(string scenePath, bool isHome)
    {
        _loadingScreen.FadeIn();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scenePath);

        asyncLoad.allowSceneActivation = false; // Prevent automatic activation

        // Wait for the delay


        yield return new WaitForSecondsRealtime(_levelTransitionDelay);

        // Allow scene activation after the delay
        asyncLoad.allowSceneActivation = true;


        while (!asyncLoad.isDone)
        {
            yield return null;
            if (!isHome)
                LevelLoaded();
            else
            {
                Time.timeScale = 1;
            }
            GetCanvas();
            _loadingScreen.FadeOut();
        }
    }

    private void LevelLoaded()
    {
        _playerController = PlayerController.instance;

        if (uesceneReset != null)
            uesceneReset.Invoke();
    }

    public void ToHome()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _playerController.enabled = false;
        string path = "Scenes/MainMenu";
        StartCoroutine(SceneLoad(path, true));
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
