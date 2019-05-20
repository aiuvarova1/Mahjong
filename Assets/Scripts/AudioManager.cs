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


    public List<AudioClip> combSounds = new List<AudioClip>();
    
    public AudioClip playingClip;

    public float soundValue = 0.5f;
    public float musicValue = 0.7f;

    public static AudioManager instance;

    //initialization
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

    
    //changes music theme in lobby/game
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


    // restores references to scene gameobjects e.g. sliders
    public void SetReferences()
    {

        if (soundSlider != null)
            return;

        settingsPanel = LocalizationManager.instance.settingsPanel;

        soundSlider = settingsPanel.GetComponentsInChildren<Slider>()[0];
        musicSlider = settingsPanel.GetComponentsInChildren<Slider>()[1];

        soundSlider.onValueChanged.AddListener(delegate { SetSound(soundSlider.value); });
        musicSlider.onValueChanged.AddListener(delegate { SetMusic(musicSlider.value); });

        SetSliders();

        mixer.SetFloat("musicVol", Mathf.Log(musicValue) * 20);
        mixer.SetFloat("soundVol", Mathf.Log(soundValue) * 20);
    }


    //sets sliders' values
    public void SetSliders()
    {
        musicSlider.value = musicValue;
        soundSlider.value = soundValue;  
    }

    // Start is called before the first frame update
    void Start()
    {
        playingClip = GetComponent<AudioSource>().clip;
    }

    //sets music sound to given float
    public void SetMusic(float soundLevel)
    {
        mixer.SetFloat("musicVol", Mathf.Log(soundLevel) * 20);
        musicValue = soundLevel;
    }

    //sets sounds' sound to given float
    public void SetSound(float soundLevel)
    {
        mixer.SetFloat("soundVol", Mathf.Log(soundLevel) * 20);
        soundValue = soundLevel;
    }

}
