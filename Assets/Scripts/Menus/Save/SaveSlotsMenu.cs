using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Database;
using System.IO;
using Unity.VisualScripting;

public enum MenuMode
{
    NewGame,
    LoadGame,
    SaveGame
}

public class SaveSlotsMenu : MonoBehaviour
{
    [SerializeField] private ConfirmationDialog confirmationDialog;

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
        if (authScript.GetUserId() == null)
        {
            authScript.showLogMsg("Log in if you want to upload a game");
            return;
        }
        if (currentSaveSlot == null)
        {
            authScript.showLogMsg("Current game has no name attached to it, consider uploading another game");
            return;
        }
        

            string profileId = currentSaveSlot.GetProfileId();
        string localFile = Path.Combine(Application.persistentDataPath, profileId, "data.json");
        if (File.Exists(localFile))
        {
            // Leer primero el contenido del archivo
            string encryptedJsonData = File.ReadAllText(localFile);
            // Desencriptar los datos
            string jsonData = fileDataHandler.EncryptDecrypt(encryptedJsonData);

            var dbReference = FirebaseDatabase.DefaultInstance.RootReference;

            string gameName = currentSaveSlot.GetGameName();

            if (string.IsNullOrEmpty(gameName))
            {
                authScript.showLogMsg("Game needs a name to be uploaded");
                return;
            }

            // Verificar si ya existe una partida con el mismo nombre
            dbReference.Child("GameData").Child(authScript.GetUserId()).Child(gameName).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error checking data: " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        Debug.Log("A save with this name already exists. Showing confirmation dialog.");

                        // Aquí llamamos a Show en el hilo principal
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            confirmationDialog.Show("A save with this name already exists. Do you want to overwrite it?",
                            () => {
                                
                                UploadSaveDataToFirebase(jsonData, gameName); // Subir los datos si el usuario confirma
                                authScript.showLogMsg("Game uploaded correctly");

                            },
                            () => {
                                authScript.showLogMsg("Upload cancelled"); // Cancelar la subida si el usuario cancela
                            });
                        });
                    }
                    else
                    {
                        Debug.Log("No existing save found. Uploading data...");
                        UploadSaveDataToFirebase(jsonData, gameName); // Subir los datos si no existe la partida
                    }
                }
            });
        }
        else
        {
            Debug.LogError("File not found: " + localFile);
        }
    }

    private void UploadSaveDataToFirebase(string jsonData, string gameName)
    {
        var dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        var profileRef = dbReference.Child("GameData").Child(authScript.GetUserId()).Child(gameName);

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

    public void OnSaveSlotClicked(SaveSlotScript saveSlot)
    {
        if (isNewGame.Equals("save"))
        {

            confirmationDialog.Show("A save with this name already exists. Do you want to overwrite it?",
                            () => {

                                DataPersistenceManager.instance.OverrideProfileId(saveSlot.GetProfileId());
                                currentSaveSlot = saveSlot;
                                UpdateSaveSlots();
                                authScript.showLogMsg("Game saved");

                            },
                            () => {
                                authScript.showLogMsg("Save cancelled"); // Cancelar la subida si el usuario cancela
                            });

            
        }

        if (isNewGame.Equals("load"))
        {
            string name = saveSlot.GetGameName();
            confirmationDialog.Show("Do you want to load " + name + "?",
                            () => {

                                DisableMenuButtons();
                                DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());

                                // Sube el archivo a Firebase
                                //UploadSaveDataToFirebase(saveSlot.GetProfileId());
                                currentSaveSlot = saveSlot;

                                levelsMenu.SetActive(true);
                                DeActivateMenu();
                                authScript.showLogMsg("Game loaded");

                            },
                            () => {
                                authScript.showLogMsg("Load cancelled"); // Cancelar la subida si el usuario cancela
                            });

           
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
