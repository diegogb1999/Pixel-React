using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
    public static MusicController instance;
    private AudioSource audioSource;
    public AudioClip startMenuMusic;
    public AudioClip level1Music;
    public AudioClip level2Music;
    public AudioClip level3Music;

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            SceneManager.sceneLoaded += OnSceneLoaded;  // Suscribirse al evento de carga de escenas

        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

     void PlayMusicForScene(string sceneName)
    {
        AudioClip clip = GetAudioClipForScene(sceneName);
        if (clip != null)
        {
            audioSource.clip = clip;

            if (sceneName == "Level 3")
            {
                audioSource.volume = 0.5f;
            }
            else
            {
                audioSource.volume = 0.33f;
            }

            audioSource.Play();
        }
    }

    AudioClip GetAudioClipForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Start Menu":
                return startMenuMusic;
            case "Level 1":
                return level1Music;
            case "Level 2":
                return level2Music;
            case "Level 3":
                return level3Music;
            default:
                return null;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;  // Desuscribirse cuando el objeto sea destruido
    }
}
