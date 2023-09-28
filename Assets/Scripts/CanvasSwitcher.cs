using System;
using System.Linq;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSwitcher : MonoBehaviour
{
    [SerializeField] private CanvasBinder[] _canvasBinders;
    private Canvas _activeCanvas;

    [Inject] private ButtonsHandler _buttonsHandler;

    // Start is called before the first frame update
    void Start()
    {
        _activeCanvas = FindObjectsByType<Canvas>(FindObjectsSortMode.None)
            .First(canvas => canvas.gameObject.activeSelf);


        foreach (var canvasBinder in _canvasBinders)
        {
            _buttonsHandler.AddHandler(canvasBinder.Button.name, (async (button, canvas) =>
            {
                _activeCanvas.gameObject.SetActive(false);
                canvasBinder.Canvas.gameObject.SetActive(true);
                _activeCanvas = canvasBinder.Canvas;

                await Awaitable.NextFrameAsync();
            }));
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}

[Serializable]
public class CanvasBinder
{
    public Canvas Canvas;
    public Button Button;
}