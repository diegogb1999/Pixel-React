using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPointScript : MonoBehaviour, IDataPersistence
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
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == GameManager.instance.levelsUnlocked)
        {
            DataPersistenceManager.instance.UnlockLevel();

            //GameManager.instance.levelsUnlocked++;
            //DataPersistenceManager.instance.gameData.levelsUnlocked = GameManager.instance.levelsUnlocked;
            //DataPersistenceManager.instance.SaveGame(); // Guardar el juego cada vez que se desbloquee un nuevo nivel
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

    public void LoadData(GameData data)
    {
        GameManager.instance.levelsUnlocked = data.levelsUnlocked;
        //GameManager.instance.gameName = data.gameName;
        //GameManager.instance.deathCount = data.deathCount;
    }

    public void SaveData(GameData data)
    {
        data.levelsUnlocked = GameManager.instance.levelsUnlocked;
        //data.gameName = GameManager.instance.gameName;
        //data.deathCount = GameManager.instance.deathCount;
    }
}
