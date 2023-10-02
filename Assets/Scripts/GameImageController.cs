using System;
using UnityEngine;
using UnityEngine.UI;

public class GameImageController : MonoBehaviour
{
    [SerializeField] private RawImage _gameImage;
    private Vector3 _imageDefaultPosition;

    [SerializeField] private Vector3 _bezie;
    [SerializeField] private float _animationRate;

    // Start is called before the first frame update
    void Start()
    {
        _imageDefaultPosition = _gameImage.transform.localPosition;
    }

    public void ResetImage()
    {
        _gameImage.transform.localPosition = _imageDefaultPosition;
    }

    public async Awaitable ShowCharacter(float y)
    {
        //_gameImage.GetComponent<AspectRatioFitter>().enabled = false;

        var localPosition = _gameImage.transform.localPosition;
        var localScale = _gameImage.transform.localScale;

        var endPosition = localPosition;
        endPosition.y -= y;

        var time = 0.0f;
        while (time <= 1.0f)
        {
            time += Bezie(_bezie.x, _bezie.y, _bezie.z, time);
            _gameImage.transform.localPosition = Vector3.Lerp(localPosition, endPosition, time);
            _gameImage.transform.localScale = Vector3.Lerp(localScale, new Vector3(1, 1, 1), time);
            await Awaitable.WaitForSecondsAsync(_animationRate);
        }
    }

    public async Awaitable FocusOnLegs()
    {
        return;
        var primaryPosition = _gameImage.transform.localPosition;

        _gameImage.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        var position = _gameImage.transform.localPosition;

        position.y += (primaryPosition.y + (primaryPosition.y * 1.3f) / 4 - primaryPosition.y);


        // Debug.Log("DeltaY: " + (secondY - firstY));
        // position.y -= secondY - firstY;
        //position.y = 442;

        _gameImage.transform.localPosition = position;
    }


    private void Update()
    {
        Debug.Log(_gameImage.GetComponent<RectTransform>().sizeDelta);
    }

    private float Bezie(float p1, float p2, float p3, float t)
    {
        return (float)(Math.Pow(1.0f - t, 2) * p1 + 2 * t * (1 - t) * p2 + Math.Pow(t, 2) * p3);
    }
}