using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private CanvasBinder[] _canvasBinders;
    private List<(string buttonName, Action<Button, Canvas> action)> _handlers = new();

    private Canvas _activeCanvas;

    private void Start()
    {
        _activeCanvas = FindObjectsByType<Canvas>(FindObjectsSortMode.None)
            .First(canvas => canvas.gameObject.activeSelf);

        foreach (var canvasBinder in _canvasBinders)
        {
            Debug.Log("Add subscriber for " + canvasBinder.Button.gameObject.name);
            canvasBinder.Button.onClick.AddListener((() =>
            {
                Debug.Log("Call :: " + canvasBinder.Button.name);

                _activeCanvas.gameObject.SetActive(false);
                canvasBinder.Canvas.gameObject.SetActive(true);
                _activeCanvas = canvasBinder.Canvas;
                
                HandleButtonClick(canvasBinder.Button, canvasBinder.Canvas);
            }));
        }
    }

    private void HandleButtonClick(Button button, Canvas canvas)
    {
        foreach (var valueTuple in _handlers)
        {
            if (button.gameObject.name == valueTuple.buttonName)
            {
                valueTuple.action(button, canvas);
            }
        }
    }

    public void AddHandler(string buttonName, Action<Button, Canvas> action)
    {
        _handlers.Add((buttonName, action));
    }
}

[Serializable]
public class CanvasBinder
{
    public Canvas Canvas;
    public Button Button;
}