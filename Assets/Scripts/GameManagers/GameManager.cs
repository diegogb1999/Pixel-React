using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private Animator transitionAnim;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(int? sceneIndex = null)
    {
        StartCoroutine(LoadLevel(sceneIndex));
    }

    IEnumerator LoadLevel(int? sceneIndex = null)
    {
        transitionAnim.SetTrigger("End");

        yield return new WaitForSeconds(2);

        int loadSceneIndex = sceneIndex ?? SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadSceneAsync(loadSceneIndex);

        transitionAnim.SetTrigger("Start");
    }

    /*public void NextLevel()
    {
        StartCoroutine(LoadNextLevel());
    }

    public void LoadScene(int scene)
    {
        StartCoroutine(LoadCustomLevel(scene));
    }

    IEnumerator LoadNextLevel()
    {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(2);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        transitionAnim.SetTrigger("Start");
    }

    IEnumerator LoadCustomLevel(int scene)
    {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(2);
        SceneManager.LoadSceneAsync(scene);
        transitionAnim.SetTrigger("Start");
    }*/
}
