using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;

    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton;

    [SerializeField] private Button saveGameButton;

    [SerializeField] private Button loadGameButton;

    [SerializeField] private InputFieldScript inputFieldScript;


    public void OnClickStartGame()
    {
        this.gameObject.SetActive(false);
        inputFieldScript.gameObject.SetActive(true);
    }

    public void OnLoadClicked()
    {
        saveSlotsMenu.ActivateMenu(true, "load");
        this.DeActivateMenu();
    }

    public void OnSaveClicked()
    {
        saveSlotsMenu.ActivateMenu(false, "save");
        this.DeActivateMenu();
    }

    public void ActivateMenu()
    {
        this.gameObject.SetActive(true);
    }
    public void DeActivateMenu()
    {
        this.gameObject.SetActive(false);
    }
}