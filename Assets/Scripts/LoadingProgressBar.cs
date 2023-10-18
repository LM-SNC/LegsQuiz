using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingProgressBar : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    private Slider _slider;

    public delegate void ResourcesLoaded();

    public event ResourcesLoaded OnResourcesLoaded;

    [Inject] private CanvasSwitcher _canvasSwitcher;
    private int _progressItems = 0;
    private int _maxProgressItems = 0;

    private float _step = 0;

    private void Start()
    {
        _slider = gameObject.GetComponent<Slider>();
    }

    public void AddProgressItems(int count)
    {
        _progressItems += count;

        if (_progressItems > _maxProgressItems)
        {
            _maxProgressItems = _progressItems;
            _step = 1.0f / _maxProgressItems;
        }
    }

    private async Awaitable SmoothAddProgress()
    {
        float valueByIteration = _step / 10;

        for (int i = 0; i < 10; i++)
        {
            _slider.value += valueByIteration;
            await Awaitable.WaitForSecondsAsync(0.025f);
        }
    }

    public void CompleteItem()
    {
        _progressItems -= 1;
        _text.SetText($"{_maxProgressItems - _progressItems}/{_maxProgressItems}");

        SmoothAddProgress();

        if (_progressItems <= 0)
        {
            _canvasSwitcher.Switch("MainMenuCanvas");
            OnResourcesLoaded!.Invoke();
        }
    }
}