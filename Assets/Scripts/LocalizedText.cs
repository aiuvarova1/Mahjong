using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string key;

    string lang = "Eng";

    // Use this for initialization
    void Start()
    {

        ChangeText();
        if(LocalizationManager.instance!=null)
            LocalizationManager.instance.ChangeEvent += ChangeText;
    }

    void ChangeLanguage()
    {
        if (lang == "Eng")
            lang = "Ru";
        else
            lang = "Eng";
    }

    private void OnDestroy()
    {
        if(LocalizationManager.instance!=null)
            LocalizationManager.instance.ChangeEvent -= ChangeText;
    }

    void ChangeText()
    {

        Text text = GetComponent<Text>();
        key = text.text;

        if (LocalizationManager.instance!=null && lang != LocalizationManager.instance.language)
        {
            text.text = LocalizationManager.instance.GetLocalizedValue(key);
            ChangeLanguage();
            LocalizationManager.instance.ChangeFont(ref text);
        }
    }

}
