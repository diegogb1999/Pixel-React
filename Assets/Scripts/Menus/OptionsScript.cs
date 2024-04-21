using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class OptionsScript : MonoBehaviour
{
    [Header("Resolution & Graphics")]
    [SerializeField] public Dropdown graphicsDropdown;
    [SerializeField] public Dropdown resolutionDropdown;
    private readonly int[] validRefreshRates = { 60, 120, 144, 240 };
    private List<Resolution> validResolutions;

    [Header("Sound")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("PlayerPrefs & AudioMixer")]
    private readonly string graphicsLevelKey = "GraphicsLevel";
    private readonly string masterPrefKey = "MasterVolume";
    private readonly string musicPrefKey = "MusicVolume";
    private readonly string sfxPrefKey = "SFXVolume";
    private readonly string masterMixerKey = "Master";
    private readonly string musicMixerKey = "Music";
    private readonly string sfxMixerKey = "SFX";

    [Header("FPS display")]
    public Text fpsText;
    public float updateRate = 0.5f;
    private float lastUpdate = 0f;
    private float frames = 0f;
    private float fps = 0f;

    private void Start()
    {
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

    void Update()
    {
        frames++;
        float currentTime = Time.realtimeSinceStartup;
        if (currentTime - lastUpdate >= updateRate)
        {
            fps = frames / (currentTime - lastUpdate);
            fpsText.text = Mathf.RoundToInt(fps) + " FPS";
            frames = 0;
            lastUpdate = currentTime;
        }
    }

    private bool IsValid(MyRes option)
    {
        int refreshRate = option.refreshRate;
        bool result = true;

        if (refreshRate > Screen.currentResolution.refreshRateRatio.value + 1)  //sumandole 1 arreglamos los valores decimales como 59.9 sin que afecte a la condicion, y nos ahorramos una funcion de parseo compleja
        {
            result = false;
        }
        else if (!validRefreshRates.Contains(option.refreshRate))
        {
            result = false;
        }
        else if (option.height < 720 && refreshRate > 60)  //si la pantalla es de peor calidad que HD no va a tener más de 60HZ
        {
            result = false;
        }

        return result;
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
        Application.targetFrameRate = (int)resolution.refreshRateRatio.value;
    }

    public void changeQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void changeMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat(masterPrefKey, volume);
        audioMixer.SetFloat(masterMixerKey, (volume == 0 ? -80 : Mathf.Log10(volume)) * 20);
    }

    public void changeMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat(musicPrefKey, volume);
        audioMixer.SetFloat(musicMixerKey, (volume == 0 ? -80 : Mathf.Log10(volume)) * 20);
    }

    public void changeSfxVolume(float volume)
    {
        PlayerPrefs.SetFloat(sfxPrefKey, volume);
        audioMixer.SetFloat(sfxMixerKey, (volume == 0 ? -80 : Mathf.Log10(volume)) * 20);
    }

    public void InitializeSound()
    {
        float masterVolume = PlayerPrefs.GetFloat(masterPrefKey, 0.5f);
        float musicVolume = PlayerPrefs.GetFloat(musicPrefKey, 0.5f);
        float SfxVolume = PlayerPrefs.GetFloat(sfxPrefKey, 0.5f);

        PlayerPrefs.SetFloat(masterPrefKey, masterVolume);
        PlayerPrefs.SetFloat(musicPrefKey, musicVolume);
        PlayerPrefs.SetFloat(sfxPrefKey, SfxVolume);

        audioMixer.SetFloat(masterMixerKey, (masterVolume == 0 ? -80 : Mathf.Log10(masterVolume)) * 20);
        audioMixer.SetFloat(musicMixerKey, (musicVolume == 0 ? -80 : Mathf.Log10(musicVolume)) * 20);
        audioMixer.SetFloat(sfxMixerKey, (SfxVolume == 0 ? -80 : Mathf.Log10(SfxVolume)) * 20);
    }

    public void InitializeSliders()
    {
        masterSlider.value = PlayerPrefs.GetFloat(masterPrefKey, 0.5f);
        masterSlider.onValueChanged.AddListener(value =>
        {
            changeMasterVolume(value);
        });
        musicSlider.value = PlayerPrefs.GetFloat(musicPrefKey, 0.5f);
        musicSlider.onValueChanged.AddListener(value =>
        {
            changeMusicVolume(value);
        });
        sfxSlider.value = PlayerPrefs.GetFloat(sfxPrefKey, 0.5f);
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
    private readonly bool resEnabled = true;
    private readonly bool ratioEnabled = false;
    private readonly bool resNameEnabled = true;

    private static readonly Dictionary<int, string> resNames = new Dictionary<int, string>
    {
        { 480, "potato" },
        { 720, "HD" },
        { 1080, "FHD" },
        { 1440, "2K" },
        { 2160, "4K" },
        { 4320, "nasa" },
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