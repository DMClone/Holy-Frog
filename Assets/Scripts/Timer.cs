using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

// created with the help of https://youtu.be/qc7J0iei3BU?si=VGe6SZ5vnxwL2daQ
public class Timer : MonoBehaviour
{
    public TextMeshProUGUI _text;
    private TimeSpan _timePlaying;
    private bool _timerGoing;
    public float elapsedTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _text.text = "";
    }

    public void BeginTimer()
    {
        _timerGoing = true;
        elapsedTime = 0;

        StartCoroutine(UpdateTimer());
    }

    public void EndTimer()
    {
        _timerGoing = false;
    }

    IEnumerator UpdateTimer()
    {
        Debug.Log("Doing Coroutine");
        while (_timerGoing)
        {
            _text.text = GetTimerString();
            yield return null;
        }
    }

    public string GetTimerString()
    {
        elapsedTime += Time.deltaTime;
        _timePlaying = TimeSpan.FromSeconds(elapsedTime);
        string timePlayingString;
        if (elapsedTime > 60)
        {
            timePlayingString = _timePlaying.ToString("mm':'s'.'ff");
        }
        else
        {
            timePlayingString = _timePlaying.ToString("s'.'ff");
        }
        return timePlayingString;
    }
}
