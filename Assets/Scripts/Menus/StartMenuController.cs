using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    public void jugar ()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void quit()
    {
        Debug.Log("Saliendo del sistema...");
        Application.Quit();
    }
}
