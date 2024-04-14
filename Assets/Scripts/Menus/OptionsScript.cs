using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsScript : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    public Dropdown resolutionDropdown;
    public Dropdown graphicsDropdown;
    Resolution[] resolutions;
    private string graphicsLevelKey = "GraphicsLevel";

    private void Start()
    {

        int currentGraphicsLevel = PlayerPrefs.GetInt(graphicsLevelKey, 2);

        // Establecer la selección del Dropdown
        graphicsDropdown.value = currentGraphicsLevel;
        graphicsDropdown.RefreshShownValue(); // Para asegurarte de que la UI muestre la selección actualizada

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
}
