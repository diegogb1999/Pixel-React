using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    public static MusicController instance;

    [SerializeField] private AudioClip startMenuMusic;
    [SerializeField] private AudioClip level1Music;
    [SerializeField] private AudioClip level2Music;
    [SerializeField] private AudioClip level3Music;

    [SerializeField] private AudioClip defaultButtonHoverSound;

    private AudioSource audioSource;
    private AudioSource audioSourceEffect;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            AudioSource[] sources = GetComponents<AudioSource>();
            audioSource = sources[0];
            audioSourceEffect = sources[1];
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Encuentra todos los botones en la escena
        Button[] buttons = FindObjectsOfType<Button>(true);

        foreach (Button button in buttons)
        {
            // Añade un listener que llama al método PlayEffect en SoundController cuando el cursor entra en el botón
            button.onClick.AddListener(() => MusicController.instance.playEffect(MusicController.instance.defaultButtonHoverSound));
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

    public void playEffect(AudioClip clip)
    {
        audioSourceEffect.PlayOneShot(clip);

    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
