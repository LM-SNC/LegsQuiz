using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CustomDropDown : TMP_Dropdown
{
    public List<Animator> BottomButtonsAnimator = new();

    protected override void Awake()
    {
        base.Awake();


        for (int i = 0; i < gameObject.transform.parent.childCount; i++)
        {
            var child = gameObject.transform.parent.GetChild(i);
        
            if (child.name.Contains("Button"))
            {
                BottomButtonsAnimator.Add(gameObject.transform.parent.GetChild(i).gameObject
                    .GetComponent<Animator>());
            }
        }
    }

    protected override GameObject CreateDropdownList(GameObject template)
    {
        foreach (var buttonAnimator in BottomButtonsAnimator)
        {
            buttonAnimator.SetBool("DropDownOpened", true);
        }

        return base.CreateDropdownList(template);
    }

    protected override void DestroyDropdownList(GameObject dropdownList)
    {
        foreach (var buttonAnimator in BottomButtonsAnimator)
        {
            buttonAnimator.SetBool("DropDownOpened", false);
        }

        base.DestroyDropdownList(dropdownList);
    }
}