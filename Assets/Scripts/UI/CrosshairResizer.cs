using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CrosshairResizer : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private LoadingScreen _loadingScreen;
    private Slider _slider;
    private Coroutine _coroutine;

    void Start()
    {
        _loadingScreen = LoadingScreen.instance; _slider = GetComponent<Slider>();
        _loadingScreen.UpdateCrosshairSize(_slider.value);
        if (PlayerPrefs.HasKey("CrosshairSize"))
            _slider.value = PlayerPrefs.GetFloat("CrosshairSize");
    }

    public void OnSelect(BaseEventData eventData)
    {
        _loadingScreen.ShowCrosshair();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _loadingScreen.HideCrosshair();
    }


    public void ValueChanged()
    {
        _loadingScreen.UpdateCrosshairSize(_slider.value);
        if (_coroutine == null)
            _coroutine = StartCoroutine(WritePref());
        else
        {
            StopCoroutine(WritePref());
            _coroutine = StartCoroutine(WritePref());
        }
    }

    private IEnumerator WritePref()
    {
        yield return new WaitForSecondsRealtime(1);
        PlayerPrefs.SetFloat("CrosshairSize", _slider.value);
    }
}
