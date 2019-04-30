using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    //public GameObject toolTipPanel;
    public Text toolTipText;

    Vector3 mousePos;

    bool needTip = false;

    

    private void OnMouseEnter()
    {
        if (!needTip) return;
        //Debug.Log(gameObject.GetComponent<TileName>().Name);
        if(LocalizationManager.instance==null||LocalizationManager.instance.language=="Eng")
            toolTipText.text = gameObject.GetComponent<TileName>().Name;
        else
            toolTipText.text = gameObject.GetComponent<TileName>().RuName;
        //toolTipPanel.SetActive(true);
        //Debug.Log(toolTipPanel.active);
        toolTipText.enabled = true;

        mousePos = Input.mousePosition;
        mousePos.y += 30;
        toolTipText.transform.position =mousePos ;
        //toolTipPanel.transform.position = mousePos;

    }

    private void OnMouseOver()
    {
        
    }
    private void OnMouseExit()
    {
        if (!needTip) return;

        toolTipText.enabled = false;
    }
    private void Start()
    {
        
        //toolTipPanel = GameObject.FindWithTag("Player").GetComponent<PlayerUI>().toolTipPanel;
       // Debug.Log("panel" + toolTipPanel == null);
       // toolTipText =toolTipPanel.transform.GetChild(0).gameObject.GetComponent<Text>();
       toolTipText = GameObject.FindWithTag("Player").GetComponent<PlayerUI>().toolTipText;
         
        toolTipText.enabled = toolTipText.enabled;
        needTip = true;
        //toolTipPanel.SetActive(false);
        //Instantiate(toolTipPanel);
    }
}
