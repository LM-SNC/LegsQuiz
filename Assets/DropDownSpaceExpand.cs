using System;
using UnityEngine;

public class DropDownSpaceExpand : MonoBehaviour
{
    public GameObject _button;
    private RectTransform _buttonRectTransform;

    public float Speed = 0.0f;

    private void Start()
    {
        _buttonRectTransform = _button.GetComponent<RectTransform>();
    }

    private void Update()
    {
        var position = _buttonRectTransform.anchoredPosition;
        var normalPos = _button.transform.position;

        Debug.Log(position.y);
        if (gameObject.transform.childCount == 4)
        {
            if (position.y > -185)
            {
                normalPos.y -= Speed;
            }
        }
        else if (position.y < -85)
        {
            normalPos.y += Speed;
        }       

        _button.transform.position = normalPos;
    }
}