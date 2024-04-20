using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsScript : MonoBehaviour
{
    [Header("Resolution & Graphics")]
    private string graphicsLevelKey = "GraphicsLevel";
    public Dropdown graphicsDropdown;
    public Dropdown resolutionDropdown;
    private List<Resolution> validResolutions;
    private static readonly List<int> validRefreshRates = new List<int> { 60, 75, 120, 144, 240 };

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private bool IsValid(MyRes option)
    {
        int width = option.width;
        int height = option.height;
        int refreshRate = option.refreshRate;

        if (refreshRate > Screen.currentResolution.refreshRateRatio.value + 1)    //sumandole 1 arreglamos los valores decimales como 59.9 sin que afecte a la condicion, y nos ahorramos una funcion de parseo compleja
        {
            return false;
        }

        if (!validRefreshRates.Contains(option.refreshRate))
        {
            return false;
        }

        if (height < 720) //si la pantalla es de peor calidad que HD no va a tener más de 60HZ
        {
            if (refreshRate > 60)
                return false;
        }

        return true;
    }

    private void Start()
    {

        if (Screen.sleepTimeout > 5)
        {
            Debug.Log("hello");
        }


        InitializeSound();
        InitializeSliders();

        graphicsDropdown.value = PlayerPrefs.GetInt(graphicsLevelKey, 2);
        graphicsDropdown.RefreshShownValue();
        resolutionDropdown.ClearOptions();

        Resolution[] allResolutions = Screen.resolutions;
        validResolutions = new List<Resolution>();
        List<string> dropdownOptions = new List<string>();

        MyRes option;
        for (int i = allResolutions.Length - 1; i >= 0; i--)
        {
            option = new MyRes(allResolutions[i]);
            string optionText = option.ToString();
            if (IsValid(option) && !dropdownOptions.Contains(optionText))
            {
                dropdownOptions.Add(optionText);
                validResolutions.Add(allResolutions[i]);
            }
        }

        int index = 0;
        for (; index < validResolutions.Count; index++)
        {
            if (validResolutions[index].width == Screen.width && validResolutions[index].height == Screen.height && validResolutions[index].refreshRateRatio.value == Screen.currentResolution.refreshRateRatio.value)
            {
                break;
            }
        }

        resolutionDropdown.AddOptions(dropdownOptions);
        resolutionDropdown.value = index;
        resolutionDropdown.RefreshShownValue();
    }

    public void setFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Vector2Int centerOfScreen = new Vector2Int(Screen.width / 2, Screen.height / 2);
        Screen.MoveMainWindowTo(Screen.mainWindowDisplayInfo, centerOfScreen);
    }

    public void setResolution(int index)
    {
        Resolution resolution = validResolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void changeQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void changeMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat("MasterVolume", volume);
        audioMixer.SetFloat("Master", (volume == 0 ? -80 : Mathf.Log10(volume)) * 20);
    }

    public void changeMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        audioMixer.SetFloat("Music", (volume == 0 ? -80 : Mathf.Log10(volume)) * 20);
    }

    public void changeSfxVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        audioMixer.SetFloat("SFX", (volume == 0 ? -80 : Mathf.Log10(volume)) * 20);
    }

    public void InitializeSound()
    {
        float masterSliderValue = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        float musicSliderValue = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float SfxSliderValue = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        PlayerPrefs.SetFloat("MasterVolume", masterSliderValue);
        PlayerPrefs.SetFloat("MusicVolume", musicSliderValue);
        PlayerPrefs.SetFloat("SFXVolume", SfxSliderValue);

        audioMixer.SetFloat("Master", (masterSliderValue == 0 ? -80 : Mathf.Log10(masterSliderValue)) * 20);
        audioMixer.SetFloat("Music", (musicSliderValue == 0 ? -80 : Mathf.Log10(musicSliderValue)) * 20);
        audioMixer.SetFloat("SFX", (SfxSliderValue == 0 ? -80 : Mathf.Log10(SfxSliderValue)) * 20);
    }

    public void InitializeSliders()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        masterSlider.onValueChanged.AddListener(value =>
        {
            changeMasterVolume(value);
        });
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        musicSlider.onValueChanged.AddListener(value =>
        {
            changeMusicVolume(value);
        });
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        sfxSlider.onValueChanged.AddListener(value =>
        {
            changeSfxVolume(value);
        });
    }

}

public class MyRes
{
    public int width;
    public int height;
    public int refreshRate;
    public int widthRatio;
    public int heightRatio;
    public string name;

    [Header("Settings")]
    private bool resEnabled = true;
    private bool ratioEnabled = false;
    private bool resNameEnabled = true;

    private static readonly Dictionary<int, string> resNames = new Dictionary<int, string>
    {
        { 480, "potato" },
        { 720, "HD" },
        { 1080, "FHD" },
        { 1440, "2K" },
        { 2160, "4K" },
    };

    public MyRes(Resolution res)
    {
        width = res.width;
        height = res.height;
        refreshRate = DoubleToInt(res.refreshRateRatio.value);

        name = resNames.ContainsKey(height) ? resNames[height] : null;
        int gcd = GetGreatestCommonDivisor(width, height);
        widthRatio = width / gcd;
        heightRatio = height / gcd;
    }

    public override string ToString()
    {
        string res = resEnabled ? $"{width} x {height} {refreshRate}hz" : "";
        string ratio = ratioEnabled ? $"[{widthRatio}:{heightRatio}]" : "";
        string resName = resNameEnabled ? (name != null ? $"({name})" : "") : "";
        return $"{res} {ratio} {resName}";
    }

    private int DoubleToInt(double number)
    {
        return Mathf.RoundToInt((float)number);
    }

    private int GetGreatestCommonDivisor(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
}
