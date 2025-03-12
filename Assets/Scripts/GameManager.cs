using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public enum PauseSetting
{
    pause,
    resume,
    toggle
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private Timer _timerText;

    public UnityEvent ue_sceneReset;

    private GameObject _pauseScreen;
    private GameObject _finishScreen;
    private GameObject _timer;

    public bool isGamePaused = true;
    private bool _isNewRun = true;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        _timerText = GetComponent<Timer>();
        HideCursor();
        Time.timeScale = 0;
    }

    void Start()
    {
        GameObject canvas = LevelCanvas.instance.gameObject;
        _pauseScreen = canvas.transform.GetChild(0).gameObject;
        _finishScreen = canvas.transform.GetChild(1).gameObject;
        _timer = canvas.transform.GetChild(2).gameObject;
    }

    public void PauseToggle(PauseSetting pauseGame, bool enableRestartButton)
    {
        if (isGamePaused && (pauseGame == PauseSetting.resume || pauseGame == PauseSetting.toggle))
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
        else if (pauseGame == PauseSetting.pause || pauseGame == PauseSetting.toggle)
        {
            isGamePaused = true;
            Time.timeScale = 0;
            _pauseScreen.SetActive(true);
        }

        if (!enableRestartButton)
        {
            _pauseScreen.transform.GetChild(1).GetComponent<Button>().interactable = false;
        }
        else if (pauseGame == PauseSetting.toggle)
        {
            _pauseScreen.transform.GetChild(1).GetComponent<Button>().interactable = true;
        }
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartLevel()
    {
        ue_sceneReset.Invoke();
        isGamePaused = true;
        _timer.SetActive(true);
        _timerText._text.text = null;
        _pauseScreen.SetActive(true);
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
        Time.timeScale = 0;
        _timer.SetActive(false);
        _finishScreen.SetActive(true);
        _finishScreen.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = _timerText.GetTimerString();
        _finishScreen.transform.GetChild(1).GetComponent<Button>().Select();
    }
}
