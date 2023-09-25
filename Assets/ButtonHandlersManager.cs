using System;
using UnityEngine;

public class ButtonHandlersManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private CanvasManager _canvasManager;

    [SerializeField] private CustomDropDown _customDropDown;

    void Start()
    {
        _canvasManager.AddHandler("MainMenuButton", (button, canvas) =>
        {
            
        });
    }
}