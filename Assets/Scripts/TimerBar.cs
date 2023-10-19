using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
    [SerializeField] private Slider _timerBar;
    private Image _timerBarFill;

    [SerializeField] private float _timeByImage;
    [SerializeField] private float _timerBarChangeRate;


    private float _timerBarChangeValue;
    private float _colorChangeValue;

    [SerializeField] private Color _timerBarEndColor;
    private Color _timerBarStartColor;

    private float _timerWaitTime;

    private Awaitable _timeBarTask;

    private bool _timerIsStop;

    public delegate void TimerEnd();

    public event TimerEnd OnTimerEnd;

    private void Awake()
    {
        _timerBarFill = _timerBar.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
        _timerBarStartColor = _timerBarFill.color;
    }

    private void Start()
    {
        _timerWaitTime = (float)TimeSpan.FromMilliseconds(1000 / (_timerBarChangeRate / _timeByImage)).TotalSeconds;
        _timerBarChangeValue = 1 / _timerBarChangeRate;
    }


    public async Awaitable StartTimer()
    {
        _timerIsStop = false;

        while (_timerBar.value > 0.0f && !_timerIsStop)
        {
            _timerBarFill.color = Color.Lerp(_timerBarStartColor, _timerBarEndColor, Math.Abs(_timerBar.value - 1.0f));
            _timerBar.value -= _timerBarChangeValue;
            await Awaitable.WaitForSecondsAsync(_timerWaitTime);
        }

        if (!_timerIsStop)
            OnTimerEnd!.Invoke();
    }

    public void StopTimer()
    {
        _timerIsStop = true;
    }

    public void ResetTimer()
    {
        _timerBarFill.color = _timerBarStartColor;
        _timerBar.value = 1.0f;
    }
}