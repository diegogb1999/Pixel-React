using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public void quit()
    {
        Debug.Log("Saliendo del sistema...");
        Application.Quit();
    }
}
