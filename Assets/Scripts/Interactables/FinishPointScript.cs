using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPointScript : MonoBehaviour
{
    [Header("Physics and animations")]

    [SerializeField] private Animator animator;

    [Header("Audio")]

    [SerializeField] private AudioClip finishSound;

    private AudioSource audioSource;
    private bool isActivated = false;

 
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

    }

    void UnlockNewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save(); 
        }
    }

    IEnumerator WaitWhileCondition()
    {
        animator.SetTrigger("endGame");
        audioSource.PlayOneShot(finishSound);

        yield return new WaitForSeconds(5f);

        UnlockNewLevel();
        GameManager.instance.LoadScene();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActivated && collision.CompareTag("Player"))
        {
            isActivated = true;
            StartCoroutine(WaitWhileCondition());           
        }
 
    }
}
