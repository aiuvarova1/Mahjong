using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mixer;

    GameObject settingsPanel;

    Slider soundSlider;
    Slider musicSlider;

    public AudioClip lobbyTheme;
    public AudioClip gameTheme;

    public AudioClip playingClip;

    float soundValue = 0.5f;
    float musicValue = 0.7f;

    public static AudioManager instance;

    private void Awake()
    {

        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    

    public void SetTheme()
    {
        if (playingClip == lobbyTheme)
        {
            playingClip = gameTheme;
            GetComponent<AudioSource>().clip = gameTheme;
            GetComponent<AudioSource>().Play();
        }
        else
        {
            playingClip = lobbyTheme;
            GetComponent<AudioSource>().clip = lobbyTheme;
            GetComponent<AudioSource>().Play();
        }
    }

    public void SetReferences()
    {
        Debug.Log("audio");

        if (GameObject.FindGameObjectWithTag("SettingsPanel") == null)
        {
            Debug.Log(GameObject.FindGameObjectWithTag("SettingsPanel") == null);
        }

        if (soundSlider != null)
            return;

        settingsPanel = LocalizationManager.instance.settingsPanel;

        soundSlider = settingsPanel.GetComponentsInChildren<Slider>()[0];
        musicSlider = settingsPanel.GetComponentsInChildren<Slider>()[1];

        Debug.Log(musicValue);
        Debug.Log(soundValue);


        soundSlider.onValueChanged.AddListener(delegate { SetSound(soundSlider.value); });
        musicSlider.onValueChanged.AddListener(delegate { SetMusic(musicSlider.value); });

        soundSlider.value = soundValue;
        musicSlider.value = musicValue;

        mixer.SetFloat("musicVol", Mathf.Log(musicValue) * 20);
        mixer.SetFloat("soundVol", Mathf.Log(soundValue) * 20);
    }

    // Start is called before the first frame update
    void Start()
    {
        playingClip = GetComponent<AudioSource>().clip;
        //SetReferences();

    }

    public void SetMusic(float soundLevel)
    {
        mixer.SetFloat("musicVol", Mathf.Log(soundLevel) * 20);
        musicValue = soundLevel;
    }


    public void SetSound(float soundLevel)
    {
        mixer.SetFloat("soundVol", Mathf.Log(soundLevel) * 20);
        soundValue = soundLevel;
    }

}
