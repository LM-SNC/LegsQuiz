using System;
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

    public void CompleteItem()
    {
        _progressItems -= 1;
        _text.SetText($"{_maxProgressItems - _progressItems}/{_maxProgressItems}");

        _slider.value += _step;

        if (_progressItems <= 0)
        {
            _canvasSwitcher.Switch("MainMenuCanvas");
            OnResourcesLoaded!.Invoke();
        }
    }
}