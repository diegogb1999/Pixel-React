using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool disableDataPersistence = false;

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    
    private List<IDataPersistence> dataPersistenceList;
    private GameData gameData;
    private FileDataHandler dataHandler;

    private string selectedProfileId = "test";

    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (disableDataPersistence)
        {
            Debug.LogWarning("Data Persistence is currently disabled!");
        }

        this.dataHandler = new FileDataHandler (Application.persistentDataPath, fileName, useEncryption);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
    }

    public void UnlockLevel()
    {
          gameData.UnlockLevel();

    }
    public void sumDeath()
    {
        gameData.sumDeath();
    }

        public void SaveName(string name)
        {
            gameData.saveName(name);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded llamado");
        if (gameData == null)
        {
            gameData = new GameData(); // Crea nuevo si no existe
        }

        this.dataPersistenceList = FindAllDataPersistenceObjects();

        foreach (IDataPersistence dataPersistenceObject in dataPersistenceList)
        {
            dataPersistenceObject.LoadData(gameData); // Usa el gameData actual en memoria
        }
    }

    public void ChangeSelectedProfileId(string newProfileId)
    {
        this.selectedProfileId = newProfileId;

        LoadGame();
    }

    public void OverrideProfileId(string newProfileId)
    {
        this.selectedProfileId = newProfileId;

        SaveGame();
        //LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();

        foreach (IDataPersistence dataPersistenceObject in this.dataPersistenceList)
        {
            dataPersistenceObject.LoadData(gameData); // Carga el nuevo estado en todos los sistemas
        }
    }

    public void LoadGame()
    {
        if (disableDataPersistence) return;

        this.gameData = dataHandler.Load(selectedProfileId);

        if (this.gameData == null)
        {
            NewGame();
        }
        Debug.Log($"Game loaded: {selectedProfileId} with levels unlocked = {this.gameData.levelsUnlocked}");
        Debug.Log($"Game loaded: {selectedProfileId} with deathCount unlocked = {this.gameData.deathCount}");
        Debug.Log($"Game loaded: {selectedProfileId} with gameName unlocked = {this.gameData.gameName}");

        foreach (IDataPersistence dataPersistenceObject in this.dataPersistenceList)
        {
            dataPersistenceObject.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        if (disableDataPersistence) return;

        if (this.gameData == null) {  return; }

        foreach (IDataPersistence dataPersistenceObject in this.dataPersistenceList)
        {
            dataPersistenceObject.SaveData(gameData);
        }
        dataHandler.Save(gameData, selectedProfileId);

        //LoadGame();
    }

    public List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceList = FindObjectsOfType<MonoBehaviour>(true).OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceList);
    }

    public bool HasGameData()
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }
}
