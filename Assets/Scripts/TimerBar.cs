using System;
using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
    [SerializeField] private Slider _timerBar;
    [SerializeField] private Image _timerBarFill;

    [SerializeField] private float _timeByImage;
    [SerializeField] private float _timerBarChangeRate;
    private float _timerBarChangeValue;
    private float _colorChangeValue;
    
    [SerializeField] private Color _timerBarEndColor;
    private Color _timerBarStartColor;

    private float _timerWaitTime;

    private void Start()
    {
        _timerWaitTime = (float)TimeSpan.FromMilliseconds(1000 / (_timerBarChangeRate / _timeByImage)).TotalSeconds;
        _timerBarChangeValue = 1 / _timerBarChangeRate;
    }

    public async Awaitable StartTimer()
    {
        _timerBarStartColor = _timerBarFill.color;
        while (_timerBar.value > 0)
        {
            _timerBarFill.color = Color.Lerp(_timerBarStartColor, _timerBarEndColor, Math.Abs(_timerBar.value - 1.0f));
            _timerBar.value -= _timerBarChangeValue;
            await Awaitable.WaitForSecondsAsync(_timerWaitTime);
        }
    }

    public void ResetTimer()
    {
        _timerBarFill.color = _timerBarStartColor;
        _timerBar.value = 1.0f;
    }
}