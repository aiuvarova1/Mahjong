﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{

    public Text toolTipText;
    Vector3 mousePos;

    bool needTip = false;

    //shows tooltip onmouseenter if needed
    private void OnMouseEnter()
    {
        if (!needTip) return;

        if (LocalizationManager.instance == null || LocalizationManager.instance.language == "Eng")

            toolTipText.text = gameObject.GetComponent<TileName>().Name;
        else
            toolTipText.text = gameObject.GetComponent<TileName>().RuName;
        if (LocalizationManager.instance != null)
            LocalizationManager.instance.ChangeFont(ref toolTipText);

        toolTipText.enabled = true;

        mousePos = Input.mousePosition;
        mousePos.y += 30;
        toolTipText.transform.position = mousePos;
    }

    //hides tooltip
    private void OnMouseExit()
    {
        if (!needTip) return;

        toolTipText.enabled = false;
    }

    //initialization
    private void Start()
    {
        toolTipText = GameObject.FindWithTag("Player").GetComponent<PlayerUI>().toolTipText;

        toolTipText.enabled = toolTipText.enabled;
        needTip = true;
    }
}
