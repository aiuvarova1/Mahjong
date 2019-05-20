using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 60;

    }

    //adds onload event
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //resets references when scene loades
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (scene.name == "Menu" && LocalizationManager.instance != null)
        {
            LocalizationManager.instance.SetReferences();
        }

        if (scene.name == "Menu" && AudioManager.instance != null)
        {
            AudioManager.instance.SetReferences();
        }

        if (scene.name == "Main")
        {
            AudioManager.instance.SetTheme();
        }

        if (scene.name == "Lobby" && AudioManager.instance.playingClip != AudioManager.instance.lobbyTheme)
            AudioManager.instance.SetTheme();
    }

    //unregisters onsceneload from event
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //leaves app
    public void Quit()
    {
        Application.Quit();
    }

    //back from lobby
    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    //go to lobby
    public void Play()
    {
        SceneManager.LoadScene("Lobby");

    }

    //loads scene async
    IEnumerator LoadAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

}

