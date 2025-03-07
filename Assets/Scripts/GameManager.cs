using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum PauseSetting
{
    pause,
    resume,
    toggle
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public UnityEvent ue_sceneReset;

    private GameObject _pauseScreen;

    private bool _isGamePaused = true;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        HideCursor();
        Time.timeScale = 0;
    }

    void Start()
    {
        GameObject canvas = LevelCanvas.instance.gameObject;
        _pauseScreen = canvas.transform.GetChild(0).gameObject;
    }

    public void PauseToggle(PauseSetting pauseGame, bool enableRestartButton)
    {
        if (_isGamePaused && (pauseGame == PauseSetting.resume || pauseGame == PauseSetting.toggle))
        {
            _isGamePaused = false;
            HideCursor();
            Time.timeScale = 1;
            _pauseScreen.gameObject.SetActive(false);
        }
        else if (pauseGame == PauseSetting.pause || pauseGame == PauseSetting.toggle)
        {
            _isGamePaused = true;
            ShowCursor();
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

    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartLevel()
    {
        ue_sceneReset.Invoke();
        _pauseScreen.transform.GetChild(1).GetComponent<Button>().interactable = false;
        _pauseScreen.transform.GetChild(0).GetComponent<Button>().Select();
    }
}
