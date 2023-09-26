using System;
using System.Collections.Generic;
using Reflex.Core;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsHandler : MonoBehaviour, IStartable
{
    private List<(string buttonName, Action<Button, Canvas> action)> _handlers = new();

    public void Start()
    {
        var allButtons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var button in allButtons)
        {
            var canvas = button.transform.root.GetComponent<Canvas>();

            Debug.Log($"Add subscriber for {button.name}");

            button.onClick.AddListener((() =>
            {
                Debug.Log($"Call :: {button.name}");

                // _activeCanvas.gameObject.SetActive(false);
                // canvasBinder.Canvas.gameObject.SetActive(true);
                // _activeCanvas = canvasBinder.Canvas;

                HandleButtonClick(button, canvas);
            }));
        }
    }

    private void HandleButtonClick(Button button, Canvas canvas)
    {
        foreach (var valueTuple in _handlers)
        {
            if (string.IsNullOrEmpty(valueTuple.buttonName) || button.gameObject.name == valueTuple.buttonName)
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