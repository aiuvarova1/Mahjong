using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Application.targetFrameRate = 60;
       
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
        Debug.Log("onsceneloaded");
        Debug.Log(scene.name);
        if (scene.name == "Menu" && LocalizationManager.instance != null)
        {
            LocalizationManager.instance.SetReferences();
        }

        if (scene.name == "Menu" && AudioManager.instance != null)
        {
            AudioManager.instance.SetReferences();
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void Quit()
    {
        Debug.Log("pressed exit");
        Application.Quit();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void Play()
    {
        SceneManager.LoadScene("Lobby");

    }
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

    // Update is called once per frame
    void Update () {
            
	}
}

