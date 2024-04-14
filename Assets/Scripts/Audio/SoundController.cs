using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    /*public static SoundController Instance;

    private AudioSource sfxSource;
    private AudioSource runningSource;

    private void Awake()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        sfxSource = sources[0];
        runningSource = sources[1];

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void playSound (AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayLoopingSound(AudioClip clip)
    {
            runningSource.clip = clip;
            runningSource.Play();
    }

    public void StopLoopingSound()
    {
        runningSource.Stop();
    }*/

}
