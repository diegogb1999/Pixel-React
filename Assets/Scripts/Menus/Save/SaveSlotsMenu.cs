using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Database;
using System.IO;

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
    [SerializeField] private GameObject saveGameText;
    [SerializeField] private GameObject loadTitle;
    [SerializeField] private GameObject levelsMenu;

    private SaveSlotScript[] saveSlots;

    private FileDataHandler fileDataHandler;

    [SerializeField] private Button backButton;

    private bool isLoadingGame = false;

    private string isNewGame = "";

    private string userFolder;

    private SaveSlotScript currentSaveSlot;

    public EmailPassLogin authScript;

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlotScript>();
        fileDataHandler = new FileDataHandler(Application.persistentDataPath, "data.json", true);
        InitializeUserFolder();
    }

    private void InitializeUserFolder()
    {
        // Verifica si ya existe un UserFolder ID almacenado
        if (PlayerPrefs.HasKey("UserFolderId"))
        {
            userFolder = PlayerPrefs.GetString("UserFolderId");
        }
        else
        {
            // Si no, genera uno nuevo, guárdalo y úsalo
            userFolder = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("UserFolderId", userFolder);
            PlayerPrefs.Save();
        }
    }

    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        this.DeActivateMenu();
    }

    private void UploadSaveDataToFirebase()
    {
        if (authScript.GetUserId() == null || currentSaveSlot == null) return;

        string profileId = currentSaveSlot.GetProfileId();
        string localFile = Path.Combine(Application.persistentDataPath, profileId, "data.json");
        if (File.Exists(localFile))
        {

            // Leer primero el contenido del archivo
            string encryptedJsonData = File.ReadAllText(localFile);
            // Desencriptar los datos
            string jsonData = fileDataHandler.EncryptDecrypt(encryptedJsonData);

            var dbReference = FirebaseDatabase.DefaultInstance.RootReference;

            // Aquí 'saves' es el nodo en la base de datos donde se guardarán los datos
            // Puedes cambiar 'saves' por el nombre del nodo que prefieras
            var profileRef = dbReference.Child("GameData").Child(authScript.GetUserId()).Child(profileId);

            profileRef.SetRawJsonValueAsync(jsonData).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error uploading data: " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Data uploaded successfully.");
                }
            });
        }
        else
        {
            Debug.LogError("File not found: " + localFile);
        }
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

            // Sube el archivo a Firebase
            //UploadSaveDataToFirebase(saveSlot.GetProfileId());
            currentSaveSlot = saveSlot;

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
            loadTitle.SetActive(false);
            
        }
        else if (isNewGame.Equals("save"))
        {
            saveGameText.SetActive(true);
            loadTitle.SetActive(false);
        }
        else if (isNewGame.Equals("load"))
        {
            saveGameText.SetActive(false);
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
