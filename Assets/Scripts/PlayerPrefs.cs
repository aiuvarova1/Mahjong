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
            if (value == "" || value.Length > 8)
                throw new ArgumentException();
            playerName = value;
            nameText.text = playerName;

        }
    }


    string path; 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            path= Application.dataPath + "/playerPrefs.txt";
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SetReferences()
    {
        if (GameObject.FindGameObjectWithTag("NamePanel") == null) return;

        namePanel = (GameObject.FindGameObjectWithTag("NamePanel"));

        if (Name != "")
            namePanel.SetActive(false);
        nameText.text = Name;
    }
    // Start is called before the first frame update
    void Start()
    {
        // nameText = (GameObject.FindGameObjectWithTag("Name")).GetComponent<Text>();
        namePanel = (GameObject.FindGameObjectWithTag("NamePanel"));


        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                int i = 0;

                while (!sr.EndOfStream)
                {

                    string line = sr.ReadLine();
                    Debug.Log(line);

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

        //catch (Exception)
        //{
        //    Debug.Log("ex");
        //    if (Name != "")
        //    {
        //        namePanel.SetActive(false);
        //    }

        //    return;
        //}

        if (Name != "")
        {
            namePanel.SetActive(false);
        }
    }

    public void EnterFirstName()
    {
        string text = (GameObject.FindGameObjectWithTag("Name")).GetComponent<Text>().text;

        ChangeName(text);

        namePanel.SetActive(false);



    }

    public void WriteFileData()
    {
        Debug.Log("write");
        try
        {
            //File.SetAttributes(path, FileAttributes.Normal);
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine(LocalizationManager.instance.language);
                sw.WriteLine(AudioManager.instance.soundValue);
                sw.WriteLine(AudioManager.instance.musicValue);
                sw.WriteLine(Name);

                

            }

            //File.SetAttributes(path, FileAttributes.Hidden);
            //File.SetAttributes(path, FileAttributes.ReadOnly);

        }
        catch(Exception ex)
        {
            return;
        }

    }

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
