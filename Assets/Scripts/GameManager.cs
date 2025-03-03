using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public UnityEvent ue_sceneReset;

    [SerializeField] private GameObject _canvas;

    private bool _isGamePaused;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        HideCursor();
    }

    void Start()
    {
        _canvas = LevelCanvas.instance.gameObject;
    }

    public void PauseToggle()
    {
        if (_isGamePaused)
        {
            _isGamePaused = false;
            HideCursor();
            Time.timeScale = 1;
            _canvas.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            _isGamePaused = true;
            ShowCursor();
            Time.timeScale = 0;
            _canvas.transform.GetChild(0).gameObject.SetActive(true);
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
    }
}
