using System;
using System.Linq;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSwitcher : MonoBehaviour
{
    [SerializeField] private CanvasBinder[] _canvasBinders;
    public Canvas ActiveCanvas { get; private set; }

    [Inject] private ButtonsHandler _buttonsHandler;

    // Start is called before the first frame update
    void Start()
    {
        ActiveCanvas = FindObjectsByType<Canvas>(FindObjectsSortMode.None)
            .First(canvas => canvas.gameObject.activeSelf);


        foreach (var canvasBinder in _canvasBinders)
        {
            _buttonsHandler.AddHandler(canvasBinder.Button.name, (async (button, canvas) =>
            {
                ActiveCanvas.gameObject.SetActive(false);
                canvasBinder.Canvas.gameObject.SetActive(true);
                ActiveCanvas = canvasBinder.Canvas;

                await Awaitable.NextFrameAsync();
            }));
        }
    }
}

[Serializable]
public class CanvasBinder
{
    public Canvas Canvas;
    public Button Button;
}