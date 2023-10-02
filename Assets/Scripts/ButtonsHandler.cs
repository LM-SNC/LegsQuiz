using System;
using System.Collections.Generic;
using Reflex.Core;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsHandler : MonoBehaviour, IStartable
{
    private List<(string buttonName, Func<Button, Canvas, Awaitable> action)> _handlers = new();

    public void Start()
    {
        var allButtons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (var button in allButtons)
        {
            if (button.tag == "UnListenable")
                continue;

            var canvas = button.transform.root.GetComponent<Canvas>();

            Debug.Log($"Add subscriber for {button.name}");

            button.onClick.AddListener((async () =>
            {
                Debug.Log($"Call :: {button.name}");

                // _activeCanvas.gameObject.SetActive(false);
                // canvasBinder.Canvas.gameObject.SetActive(true);
                // _activeCanvas = canvasBinder.Canvas;

                await HandleButtonClick(button, canvas);
            }));
        }
    }

    private async Awaitable HandleButtonClick(Button button, Canvas canvas)
    {
        foreach (var valueTuple in _handlers)
        {
            if (string.IsNullOrEmpty(valueTuple.buttonName) || button.gameObject.name == valueTuple.buttonName)
            {
                await valueTuple.action(button, canvas);
            }
        }
    }

    public void AddHandler(string buttonName, Func<Button, Canvas, Awaitable> action)
    {
        _handlers.Add((buttonName, action));
    }
}