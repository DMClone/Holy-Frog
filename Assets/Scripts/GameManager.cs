using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public enum PauseSetting
{
    Pause,
    Resume,
    Toggle
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("Dependencies")]
    [SerializeField] private GameObject _pauseScreen;
    [SerializeField] private GameObject _finishScreen;
    [SerializeField] private GameObject _timer;
    [SerializeField] private Timer _timerText;

    [Header("Settings")]
    [Tooltip("Will start indication 10 units up")] public int killDepth;
    private bool _isNewRun = true;
    [HideInInspector] public bool isGamePaused = true;

    [HideInInspector] public UnityEvent ue_sceneReset;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        _timerText = GetComponent<Timer>();
        HideCursor();
    }

    void Start()
    {
        GameObject canvas = LevelCanvas.instance.gameObject;
        _pauseScreen = canvas.transform.GetChild(0).gameObject;
        _finishScreen = canvas.transform.GetChild(1).gameObject;
        _timer = canvas.transform.GetChild(2).gameObject;

        PlayerController.instance.gameManager = this;
        Time.timeScale = 0;
    }

    public void PauseToggle(PauseSetting pauseGame, bool enableRestartButton)
    {
        if (!_finishScreen.activeSelf)
        {
            if (isGamePaused && (pauseGame == PauseSetting.Resume || pauseGame == PauseSetting.Toggle))
            {
                isGamePaused = false;
                HideCursor();
                Time.timeScale = 1;
                _pauseScreen.gameObject.SetActive(false);

                if (_isNewRun)
                {
                    _timerText.BeginTimer();
                    _isNewRun = false;
                }
            }
            else if (pauseGame == PauseSetting.Pause || pauseGame == PauseSetting.Toggle)
            {
                isGamePaused = true;
                Time.timeScale = 0;
                _pauseScreen.SetActive(true);
            }

            if (!enableRestartButton)
            {
                _pauseScreen.transform.GetChild(1).GetComponent<Button>().interactable = false;
            }
            else if (pauseGame == PauseSetting.Toggle)
            {
                _pauseScreen.transform.GetChild(1).GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            _finishScreen.SetActive(false);
            RestartLevel();
        }
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        ue_sceneReset.Invoke();
        Time.timeScale = 0;
        isGamePaused = true;
        _timer?.SetActive(true);
        _timerText._text.text = null;
        _pauseScreen?.SetActive(true);
        _pauseScreen.transform.GetChild(1).GetComponent<Button>().interactable = false;
        _pauseScreen.transform.GetChild(0).GetComponent<Button>().Select();
        _timerText.EndTimer();
        _isNewRun = true;

        if (_finishScreen.activeSelf)
        {
            _finishScreen.SetActive(false);
        }
    }

    public void Finish()
    {
        isGamePaused = true;
        Time.timeScale = 0;
        _timer.SetActive(false);
        _finishScreen.SetActive(true);
        _finishScreen.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Time: " + _timerText.elapsedTime;
        _finishScreen.transform.GetChild(1).GetComponent<Button>().Select();

        int levelUnlocked = GetLevelIndex() + 1;
        LevelManager.instance.UpdateUnlock(levelUnlocked);
    }

    private int GetLevelIndex()
    {
        int levelIndex = SceneManager.GetActiveScene().name.IndexOf("Level");
        string numberPart = SceneManager.GetActiveScene().name.Substring(levelIndex + "Level".Length);

        int.TryParse(numberPart, out int levelNumber);
        return levelNumber;
    }
}
