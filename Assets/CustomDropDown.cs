using System;
using TMPro;
using UnityEditor;
using UnityEngine;

[Serializable]
public class CustomDropDown : TMP_Dropdown
{
    private Animator _startButtonAnimator;

    protected override void Awake()
    {
        base.Awake();


        for (int i = 0; i < gameObject.transform.parent.childCount; i++)
        {
            var child = gameObject.transform.parent.GetChild(i);

            if (child.gameObject == gameObject)
            {
                _startButtonAnimator = gameObject.transform.parent.GetChild(i + 1).gameObject.GetComponent<Animator>();
                break;
            }

            // if (myIndex != -1)
            // {
            //     _startButton = child.gameObject;
            //     break;
            // }
        }
    }

    protected override GameObject CreateDropdownList(GameObject template)
    {
        _startButtonAnimator.SetBool("DropDownOpened", true);
        return base.CreateDropdownList(template);
    }

    protected override void DestroyDropdownList(GameObject dropdownList)
    {
        _startButtonAnimator.SetBool("DropDownOpened", false);
        base.DestroyDropdownList(dropdownList);
    }
}