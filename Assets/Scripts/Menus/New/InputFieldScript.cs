using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputFieldScript : MonoBehaviour
{

    [SerializeField] private GameObject levelsMenu;

    [Header("Game Name Input")]
    [SerializeField] private TMP_InputField gameNameInputField;


    public void setGameName()
    {
        if (!string.IsNullOrEmpty(gameNameInputField.text))
        {
            DataPersistenceManager.instance.NewGame();
            DataPersistenceManager.instance.SaveName(gameNameInputField.text);
            this.gameObject.SetActive(false);
            levelsMenu.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Game name is empty. Please enter a name.");
        }
    }
}
