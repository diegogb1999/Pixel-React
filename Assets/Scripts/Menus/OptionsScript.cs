using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public static class MyParser
{
    public static int DoubleToInt(double number)
    {
        return Mathf.RoundToInt((float)number);
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

    private Dictionary<int, string> resNames = new Dictionary<int, string>
    {
        { 720, "HD" },
        { 1080, "FHD" },
        { 1440, "2K" },
        { 2160, "4K" }
    };

    public MyRes(Resolution res)
    {
        this.width = res.width;
        this.height = res.height;
        this.refreshRate = MyParser.DoubleToInt(res.refreshRateRatio.value);

        this.name = resNames.ContainsKey(height) ? resNames[height] : null;
        int gcd = GetGreatestCommonDivisor(width, height);
        this.widthRatio = width / gcd;
        this.heightRatio = height / gcd;
    }

    public override string ToString()
    {
        string res = $"{width} x {height} {refreshRate}hz";
        string ratio = $"[{widthRatio}:{heightRatio}]";
        string resName = name != null ? $"({name})" : "";
        return $"{res} {resName}";
    }

    private static int GetGreatestCommonDivisor(int a, int b)
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

public class OptionsScript : MonoBehaviour
{
    [Header("Resolution & Graphics")]

    public Dropdown resolutionDropdown;
    public Dropdown graphicsDropdown;

    private string graphicsLevelKey = "GraphicsLevel";
    private List<int> validRefreshRates = new List<int> { 60, 75, 120, 144, 240 };
    private List<Resolution> validResolutions;

    [Header("Audio")]

    [SerializeField] private AudioMixer audioMixer;

    private bool IsValid(MyRes option)
    {
        int width = option.width;
        int height = option.height;
        int refreshRate = option.refreshRate;

        if (refreshRate > MyParser.DoubleToInt(Screen.currentResolution.refreshRateRatio.value))
        {
            return false;
        }

        if (!validRefreshRates.Contains(option.refreshRate))
        {
            return false;
        }

        if (height < 720)
        {
            if (refreshRate > 60)
                return false;
        }

        return true;
    }

    private void Start()
    {
        int currentGraphicsLevel = PlayerPrefs.GetInt(graphicsLevelKey, 2);
        graphicsDropdown.value = currentGraphicsLevel;
        graphicsDropdown.RefreshShownValue();
        resolutionDropdown.ClearOptions();

        Resolution[] allResolutions = Screen.resolutions;
        validResolutions = new List<Resolution>();
        List<string> dropdownOptions = new List<string>();

        MyRes option;
        int currentResolutionIndex = 0;
        for (int i = allResolutions.Length - 1; i >= 0; i--)
        {
            option = new MyRes(allResolutions[i]);
            if (IsValid(option) && !dropdownOptions.Contains(option.ToString()))
            {
                dropdownOptions.Add(option.ToString());
                validResolutions.Add(allResolutions[i]);
            }
        }

        for (int i = 0; i < validResolutions.Count; i++)
        {
            if (validResolutions[i].width == Screen.width && validResolutions[i].height == Screen.height && MyParser.DoubleToInt(validResolutions[i].refreshRateRatio.value) == MyParser.DoubleToInt(Screen.currentResolution.refreshRateRatio.value))
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(dropdownOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void setResolution(int index)
    {
        Resolution resolution = validResolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void setFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void changeVolume(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }

    public void changeQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }
}

/*public class OptionsScript : MonoBehaviour
{
    [Header("Resolution & Graphics")]

    public Dropdown resolutionDropdown;
    public Dropdown graphicsDropdown;

    Resolution[] resolutions;
    private string graphicsLevelKey = "GraphicsLevel";

    [Header("Audio")]

    [SerializeField] private AudioMixer audioMixer;

    private void Start()
    {

        int currentGraphicsLevel = PlayerPrefs.GetInt(graphicsLevelKey, 2);

        graphicsDropdown.value = currentGraphicsLevel;
        graphicsDropdown.RefreshShownValue();

        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRateRatio + "hz";
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void setResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void setFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void changeVolume(float volume)
    {
        //audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        //audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }

    public void changeQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public static int DoubleToInt(double number)
    {
        return Mathf.RoundToInt((float)number);
    }
}*/
