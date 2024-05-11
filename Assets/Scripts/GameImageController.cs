using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class GameImageController : MonoBehaviour
{
    
    [SerializeField] private float _imageScale;
    private RectTransform _imageContainerRectTransform;

    public event UnityAction OnImageLoaded;

    private void Awake()
    {
       // _imageContainerRectTransform = _gameImage.transform.parent.GetComponent<RectTransform>();
    }

    public void FaceFocus()
    {
      //  _gameImage.GetComponent<Animator>().SetTrigger("move");
    }

    public void SetImage(string image)
    {
       // _gameImage.texture = null;
        DownloadImage(true, image);
    }

    public void DownloadImage(bool withCallback, string image)
    {
        var operation = Addressables.LoadAssetAsync<Texture2D>(image);
        if (withCallback)
        {
            operation.Completed += OnCurrentImageLoaded;
        }
    }

    private void OnCurrentImageLoaded(AsyncOperationHandle<Texture2D> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var texture = handle.Result;
          //  _gameImage.texture = texture;

            OnImageLoaded?.Invoke();
        }
        else
        {
            Debug.LogError("Failed to load the image");
        }
    }


    public void LegsFocus()
    {
       // _gameImage.transform.localScale = new Vector3(_imageScale, _imageScale, _imageScale);

      //  var pos = _gameImage.transform.localPosition;
      //  pos.y = (_gameImage.rectTransform.rect.height * _imageScale - _imageContainerRectTransform.rect.height) / 2;

       // _gameImage.transform.localPosition = pos;
    }
}