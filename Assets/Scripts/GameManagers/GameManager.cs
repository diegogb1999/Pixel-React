using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager instance;

    public int levelsUnlocked = 1;
    public string gameName = "";
    public int deathCount = 0;

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

        /*if (SceneManager.GetActiveScene().buildIndex == levelsUnlocked && SceneManager.GetActiveScene().buildIndex != 0)
        {
            levelsUnlocked++;
        }*/

        int loadSceneIndex = sceneIndex ?? levelsUnlocked;
        SceneManager.LoadSceneAsync(loadSceneIndex);

        transitionAnim.SetTrigger("Start");
    }

    public void LoadData(GameData data)
    {
        this.levelsUnlocked = data.levelsUnlocked;
        //this.gameName = data.gameName;
        //this.deathCount = data.deathCount;
    }

    public void SaveData(GameData data)
    {
        data.levelsUnlocked = this.levelsUnlocked;
        //data.gameName = this.gameName;
        //data.deathCount = this.deathCount;
    }

}
