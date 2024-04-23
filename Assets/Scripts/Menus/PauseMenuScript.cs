using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{
    [Header("Visual UI")]

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameUI;

    [SerializeField] private Button basicAttackButton;
    [SerializeField] private Image cdMaskBasicAttack;
    [SerializeField] private TextMeshProUGUI timerBasicAttack;

    [SerializeField] private Button eSkillButton;
    [SerializeField] private Image cdMaskEskill;
    [SerializeField] private TextMeshProUGUI timerEskill; 

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

    [Header("Player references")]

    private PlayerCombat playerCombatScript;

    void Start()
    {
        LoadSettings();

        SFXSlider.onValueChanged.AddListener(value =>
        {
            changeSFX(value);
            UpdateIcon(SFXSlider, SFXicon, SFXoff, SFXon);
        });
        MusicSlider.onValueChanged.AddListener(value =>
        {
            changeMusic(value);
            UpdateIcon(MusicSlider, MusicIcon, MusicOff, MusicOn);
        });

        UpdateIcon(SFXSlider, SFXicon, SFXoff, SFXon);
        UpdateIcon(MusicSlider, MusicIcon, MusicOff, MusicOn);

        playerCombatScript = FindAnyObjectByType<PlayerCombat>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.activeSelf)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        cdDisplay(cdMaskBasicAttack, timerBasicAttack, playerCombatScript.nextAttackTimeBasicAttack, playerCombatScript.attackRateBasicAttack);
        cdDisplay(cdMaskEskill, timerEskill, playerCombatScript.nextAttackTimeEskill, playerCombatScript.attackRateEskill);
    }

    private void cdDisplay(Image cdMask, TextMeshProUGUI timer, float nextAttackTime, float attackRate)
    {
        float timeLeft = nextAttackTime - Time.time;
        float cd = 1f / attackRate;

        if (timeLeft > 0)
        {
            timer.enabled = true;
            cdMask.enabled = true;

            timer.text = timeLeft.ToString("F1") + "s";

            cdMask.fillAmount = timeLeft / cd;
        }
        else
        {
            timer.enabled = false;
            cdMask.enabled = false;
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

    public void toggleMusic()
    {
        if (MusicIcon.sprite == MusicOff)
        {
            float value = PlayerPrefs.GetFloat("MusicVolumeTemp", 0.5f);
            changeMusic(value);
            MusicSlider.value = value;
            MusicIcon.sprite = MusicOn;
            PlayerPrefs.DeleteKey("MusicVolumeTemp");
        }
        else
        {
            PlayerPrefs.SetFloat("MusicVolumeTemp", MusicSlider.value);
            changeMusic(0);
            MusicSlider.value = 0;
            MusicIcon.sprite = MusicOff;
        }
    }

    public void toggleSfx()
    {
        if (SFXicon.sprite == SFXoff)
        {
            float value = PlayerPrefs.GetFloat("SFXVolumeTemp", 0.5f);
            changeSFX(value);
            SFXSlider.value = value;
            SFXicon.sprite = SFXon;
            PlayerPrefs.DeleteKey("SFXVolumeTemp");
        }
        else
        {
            PlayerPrefs.SetFloat("SFXVolumeTemp", SFXSlider.value);
            changeSFX(0);
            SFXSlider.value = 0;
            SFXicon.sprite = SFXoff;
        }
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
        gameUI.SetActive(false);
        Time.timeScale = 0;
        FindObjectOfType<PlayerCombat>().UpdateHealthBar();
        Mute();
    }

    public void Home()
    {
        SceneManager.LoadScene("Start Menu");
        Time.timeScale = 1;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        gameUI.SetActive(true);
        Time.timeScale = 1;
        FindObjectOfType<PlayerCombat>().UpdateHealthBar();
        Unmute();
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
