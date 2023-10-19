using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GameImageController : MonoBehaviour
{
    [SerializeField] private RawImage _gameImage;
    [SerializeField] private float _imageScale;
    private RectTransform _imageContainerRectTransform;

    [SerializeField] private Vector3 _bezie;
    [SerializeField] private float _animationRate;

    // Start is called before the first frame update

    private void Awake()
    {
        _imageContainerRectTransform = _gameImage.transform.parent.GetComponent<RectTransform>();
    }

    public async Awaitable FaceFocus(CancellationToken cancellationToken = default (CancellationToken))
    {
        var localPosition = _gameImage.transform.localPosition;
        var localScale = _gameImage.transform.localScale;

        var endPosition = localPosition;

        endPosition.y = -((_gameImage.rectTransform.rect.height - _imageContainerRectTransform.rect.height) / 2);

        var time = 0.0f;
        while (time <= 1.0f && !cancellationToken.IsCancellationRequested)
        {
            time += Bezie(_bezie.x, _bezie.y, _bezie.z, time) + 0.001f * Time.deltaTime;
            _gameImage.transform.localPosition = Vector3.Lerp(localPosition, endPosition, time);
            _gameImage.transform.localScale = Vector3.Lerp(localScale, new Vector3(1, 1, 1), time);
            try
            {
                await Awaitable.WaitForSecondsAsync(_animationRate, cancellationToken);
            }
            catch (Exception e)
            {
                // ignored
            }
        }
    }

    public void SetImage(byte[] image)
    {
        var texture = new Texture2D(1, 1);
        texture.LoadImage(image);
        _gameImage.texture = texture;
    }
    
    public void LegsFocus()
    {
        _gameImage.transform.localScale = new Vector3(_imageScale, _imageScale, _imageScale);

        var pos = _gameImage.transform.localPosition;
        pos.y = (_gameImage.rectTransform.rect.height * _imageScale - _imageContainerRectTransform.rect.height) / 2;

        _gameImage.transform.localPosition = pos;
    }

    private float Bezie(float p1, float p2, float p3, float t)
    {
        return (float)(Math.Pow(1.0f - t, 2) * p1 + 2 * t * (1 - t) * p2 + Math.Pow(t, 2) * p3);
    }
}