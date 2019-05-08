using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiWinds : MonoBehaviour
{

    public List<Text> labels;

    Dictionary<string, Text> windLabels = new Dictionary<string, Text>();

   // List<Text> windlabels = new List<Text>();

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

        if (LocalizationManager.instance != null)
        {
            labels[0].text = LocalizationManager.instance.GetLocalizedValue(left);
            labels[1].text = LocalizationManager.instance.GetLocalizedValue(up);
            labels[2].text = LocalizationManager.instance.GetLocalizedValue(right);

        }
        else
        {
            labels[0].text = left;
            labels[1].text = up;
            labels[2].text = right;
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
