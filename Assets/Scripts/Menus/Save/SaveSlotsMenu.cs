using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum MenuMode
{
    NewGame,
    LoadGame,
    SaveGame
}

public class SaveSlotsMenu : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private MainMenuScript mainMenu;
    [SerializeField] private GameObject newGameText;
    [SerializeField] private GameObject saveGameText;
    [SerializeField] private GameObject loadTitle;
    [SerializeField] private GameObject levelsMenu;

    private SaveSlotScript[] saveSlots;

    [SerializeField] private Button backButton;

    private bool isLoadingGame = false;

    private string isNewGame = "";

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlotScript>();
    }

    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        this.DeActivateMenu();
    }

    public void OnSaveSlotClicked(SaveSlotScript saveSlot)
    {
        if (isNewGame.Equals("save"))
        {
            DataPersistenceManager.instance.OverrideProfileId(saveSlot.GetProfileId());
            UpdateSaveSlots();
        }   

        if (isNewGame.Equals("load"))
        {
            DisableMenuButtons();
            DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            levelsMenu.SetActive(true);
            DeActivateMenu();
        }   
        
    }

    private void UpdateSaveSlots()
    {
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();
        foreach (SaveSlotScript saveSlot in saveSlots)
        {
            if (profilesGameData.TryGetValue(saveSlot.GetProfileId(), out GameData profileData))
            {
                saveSlot.SetData(profileData);
            }
            else
            {
                // Manejar el caso donde no hay datos (posiblemente mostrando 'No data')
                saveSlot.SetData(null);
            }
        }
    }

    public void ActivateMenu(bool isLoadingGame, string isNewGame)
    {
        this.gameObject.SetActive(true);
        backButton.interactable = true;
        this.isLoadingGame = isLoadingGame;
        this.isNewGame = isNewGame;

        if (isNewGame.Equals("new"))
        {
            saveGameText.SetActive(false);
            newGameText.SetActive(true);
            loadTitle.SetActive(false);
            
        }
        else if (isNewGame.Equals("save"))
        {
            saveGameText.SetActive(true);
            newGameText.SetActive(false);
            loadTitle.SetActive(false);
        }
        else if (isNewGame.Equals("load"))
        {
            saveGameText.SetActive(false);
            newGameText.SetActive(false);
            loadTitle.SetActive(true);
        }

        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        foreach (SaveSlotScript saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);

            if (profileData == null && isLoadingGame) 
            {
                saveSlot.setInteractable(false);
            }
            else
            {
                saveSlot.setInteractable(true);
            }
        }
    }


    private void DeActivateMenu()
    {
        this.gameObject.SetActive(false);
    }

    public void DisableMenuButtons()
    {
        foreach (SaveSlotScript saveSlot in saveSlots)
        {
            saveSlot.setInteractable(false);
        }
        backButton.interactable = false;
    }
}
