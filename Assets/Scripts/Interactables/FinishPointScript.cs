using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPointScript : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D collision;

    private AudioSource audioSource;
    [SerializeField] private AudioClip finishSound;

    private bool isActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        //enemy = GameObject.Find("Flower Enemy Pathing AB");
        animator = GetComponent<Animator>();
        collision = GetComponent<BoxCollider2D>();
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

    IEnumerator EsperarMientrasCondicion()
    {
        animator.SetTrigger("endGame");
        audioSource.PlayOneShot(finishSound);

        yield return new WaitForSeconds(5f);

        UnlockNewLevel();
        GameManager.instance.NextLevel();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActivated && collision.CompareTag("Player"))
        {
            isActivated = true;
            StartCoroutine(EsperarMientrasCondicion());           
        }
 
    }
}
