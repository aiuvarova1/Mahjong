using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UiWinds : MonoBehaviour
{

    public List<Text> labels;

    Dictionary<string, Text> windLabels = new Dictionary<string, Text>();

    public void AssignWinds()
    {
        windLabels.Clear();
        switch (gameObject.GetComponent<Player>().wind)
        {
            case "East":
                AssignLabels("North", "West", "South");
                break;
            case "South":
                AssignLabels("East", "North", "West");
                break;
            case "West":
                AssignLabels("South", "East", "North");
                break;
            case "North":
                AssignLabels("West", "South", "East");
                break;
            default:
                Debug.Log("wrong wind");
                return;
        }
    }

    void AssignLabels(string left,string up,string right)
    {
        windLabels.Add(left,labels[0]);
        windLabels.Add(up,labels[1]);
        windLabels.Add(right,labels[2]);

        List<string> names = new List<string>();

        for (int i = 0; i < 3; i++)
        {
            if (labels[i].text.Length != 0)
            {
                try
                {
                    names.Add( "\n"+(labels[i].text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))[1]);
                }
                catch (Exception ex)
                {
                    names.Add( "");
                }
            }
            else
            {
                names.Add("");
            }
        }

        if (LocalizationManager.instance != null)
        {
            labels[0].text = LocalizationManager.instance.GetLocalizedValue(left) + names[0];
            labels[1].text = LocalizationManager.instance.GetLocalizedValue(up) + names[1];
            labels[2].text = LocalizationManager.instance.GetLocalizedValue(right) + names[2];

        }
        else
        {
            labels[0].text = left + names[0];
            labels[1].text = up+ names[1];
            labels[2].text = right+names[2];
        }
    }

    public void SetName(string wind,string name)
    {
        if (windLabels.ContainsKey(wind))
            windLabels[wind].text = windLabels[wind].text + "\n" + name;
    }

    public void RemoveName(string wind)
    {
        if (windLabels.ContainsKey(wind))
            windLabels[wind].text = wind;
    }
}
