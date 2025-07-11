using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;
    private GameObject _screen;
    [SerializeField] private RectTransform _crosshair;
    public Image crosshairFill;

    private Image _image;

    [Range(0, 5)][SerializeField] private float _fadeSpeed;
    [SerializeField] private bool _fadingIn;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        _screen = transform.GetChild(0).gameObject;
        _image = _screen.GetComponent<Image>();
    }

    public void FadeIn()
    {
        _fadingIn = true;
    }
    public void FadeOut()
    {
        _fadingIn = false;
    }

    private void Update()
    {
        if (_fadingIn && _image.color.a < 1)

            _image.color += new Color(0, 0, 0, _fadeSpeed * Time.unscaledDeltaTime);
        else if (_image.color.a > 1)
            _image.color = new Color(0, 0, 0, 1);
        else if (!_fadingIn && _image.color.a > 0)
            _image.color -= new Color(0, 0, 0, _fadeSpeed * Time.unscaledDeltaTime);
        else if (_image.color.a < 0)
            _image.color = new Color(0, 0, 0, 0);
    }

    public void UpdateCrosshairSize(float scale)
    {
        _crosshair.localScale = new Vector3(scale, scale, scale);
    }

    public void ShowCrosshair()
    {
        if (_crosshair != null)
            _crosshair.gameObject.SetActive(true);
    }

    public void HideCrosshair()
    {
        if (_crosshair != null)
            _crosshair.gameObject.SetActive(false);
    }

}
