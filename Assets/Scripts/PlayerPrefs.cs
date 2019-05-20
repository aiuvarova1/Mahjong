using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;


public class PlayerPrefs : MonoBehaviour
{
    public InputField nameText;
    GameObject namePanel;

    string playerName = "";

    public static PlayerPrefs instance;

    public string Name
    {
        get { return playerName; }
        set
        {
            if (value == "" || value.Length > 10)
                throw new ArgumentException();
            playerName = value;
            nameText.text = playerName;
        }
    }

    string path;

    //initialization
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            path = Application.dataPath + "/playerPrefs.txt";
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    //sets references to gameobjects
    public void SetReferences()
    {
        if (GameObject.FindGameObjectWithTag("NamePanel") == null) return;

        namePanel = (GameObject.FindGameObjectWithTag("NamePanel"));

        if (Name != "")
            namePanel.SetActive(false);
        nameText.text = Name;
        nameText.onEndEdit.AddListener(delegate { ChangeName(nameText.text); });

    }

    // Start is called before the first frame update
    void Start()
    {
        namePanel = (GameObject.FindGameObjectWithTag("NamePanel"));
        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                int i = 0;

                while (!sr.EndOfStream)
                {

                    string line = sr.ReadLine();
                    float data;

                    switch (i)
                    {
                        case 0:
                            if (line != "Eng" && line != "Ru")
                                break;
                            if (line == "Ru")
                            {
                                LocalizationManager.instance.dropDown.value = 2;
                            }
                            break;
                        case 1:
                            if (!float.TryParse(line, out data) || data < 0.001 || data > 1)
                            {
                                break;
                            }
                            AudioManager.instance.SetSound(data);
                            AudioManager.instance.SetSliders();

                            break;
                        case 2:
                            if (!float.TryParse(line, out data) || data < 0.001 || data > 1)
                            {
                                break;
                            }
                            AudioManager.instance.SetMusic(data);
                            AudioManager.instance.SetSliders();

                            break;
                        case 3:
                            Name = line;
                            break;
                        default:
                            return;

                    }
                    i++;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("ex");
            Debug.Log(e.Message);
        }

        if (Name != "")
        {
            namePanel.SetActive(false);
        }
    }

    //controls first name entering
    public void EnterFirstName()
    {
        string text = (GameObject.FindGameObjectWithTag("Name")).GetComponent<Text>().text;

        ChangeName(text);
        namePanel.SetActive(false);
    }

    //writes player settings to file
    public void WriteFileData()
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine(LocalizationManager.instance.language);
                sw.WriteLine(AudioManager.instance.soundValue);
                sw.WriteLine(AudioManager.instance.musicValue);
                sw.WriteLine(Name);
            }
        }
        catch (Exception ex)
        {
            return;
        }
    }

    //changes name and fixes it in file
    public void ChangeName(string text)
    {
        try
        {
            Name = text;
        }
        catch (ArgumentException)
        {
            return;
        }

        WriteFileData();
    }
}
