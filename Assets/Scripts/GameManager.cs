using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private TimerBar _timerBar;
    [SerializeField] private RawImage _gameImage;
    private Vector3 _imageDefaultPosition;
    
    [SerializeField] private Vector3 _bezie;
    [SerializeField] private float _animationRate;

    // Start is called before the first frame update
    private async void Start()
    {
        _timerBar = gameObject.GetComponent<TimerBar>();
        _timerBar.StartTimer();

        _imageDefaultPosition = _gameImage.transform.localPosition;

        await UpImage(461);
        //ResetImage();
    }

    private void ResetImage()
    {
        _gameImage.transform.localPosition = _imageDefaultPosition;
    }

    private async Awaitable UpImage(float y)
    {
        //_gameImage.GetComponent<AspectRatioFitter>().enabled = false;

        var localPosition = _gameImage.transform.localPosition;

        var endPosition = localPosition;
        endPosition.y -= y;

        var time = 0.0f;
        while (time <= 1.0f)
        {
            time += Bezie(_bezie.x, _bezie.y, _bezie.z, time);
            _gameImage.transform.localPosition = Vector3.Lerp(localPosition, endPosition, time);
            await Awaitable.WaitForSecondsAsync(_animationRate);
        }
    }

    private float Bezie(float p1, float p2, float p3, float t)
    {
        return (float)(Math.Pow(1.0f - t, 2) * p1 + 2 * t * (1 - t) * p2 + Math.Pow(t, 2) * p3);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}