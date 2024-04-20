using System.Collections;
using System.Collections.Generic;
//using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{
    [Header("Visual UI")]

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gearIcon;

    [Header("Audio")]

    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Image SFXicon;
    [SerializeField] private Sprite SFXoff;
    [SerializeField] private Sprite SFXon;

    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Image MusicIcon;
    [SerializeField] private Sprite MusicOff;
    [SerializeField] private Sprite MusicOn;

    [SerializeField] private AudioMixer audioMixer;

    void Start()
    {
        LoadSettings();

        SFXSlider.onValueChanged.AddListener(value => {
            changeSFX(value);
            UpdateIcon(SFXSlider, SFXicon, SFXoff, SFXon);
        });
        MusicSlider.onValueChanged.AddListener(value => {
            changeMusic(value);
            UpdateIcon(MusicSlider, MusicIcon, MusicOff, MusicOn);
        });

        //LoadSettings();

        UpdateIcon(SFXSlider, SFXicon, SFXoff, SFXon);
        UpdateIcon(MusicSlider, MusicIcon, MusicOff, MusicOn);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.activeSelf)
            {
                gearIcon.SetActive(true);
                Resume();
                Unmute();
            }
            else
            {
                gearIcon.SetActive(false);
                Pause();
                Mute();
            }
        }
    }

    private void LoadSettings()
    {
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
    }

    private void UpdateIcon(Slider slider, Image icon, Sprite offIcon, Sprite onIcon)
    {
        icon.sprite = slider.value == 0 ? offIcon : onIcon;
    }

    public void changeSFX(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        audioMixer.SetFloat("SFX", (volume == 0 ? -80 : Mathf.Log10(volume)) * 20);
    }

    public void changeMusic(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        audioMixer.SetFloat("Music", (volume == 0 ? -80 : Mathf.Log10(volume)) * 20);
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Home()
    {
        SceneManager.LoadScene("Start Menu");
        Time.timeScale = 1;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void Mute()
    {
        AudioListener.volume = 0;
    }

    public void Unmute()
    {
        AudioListener.volume = 1;
    }
}
