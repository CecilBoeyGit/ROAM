using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{

    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    GameData gameData;
    List<IDataPersistence> dataPersistenceObjects;

    private FileDataHandler dataHandler;

    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("DataPersistence already exists!");
        }

        instance = this;
    }
    private void OnEnable()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = GetAllDataPersistenceObjects();
        LoadData();
    }

    private void Start()
    {

    }

    public void NewData()
    {
        this.gameData = new GameData();
    }
    public void LoadData()
    {
        //Try read gameData from the DataHandler reading from the file directory
        this.gameData = dataHandler.Load();

        if(this.gameData == null)
        {
            NewData();
        }

        foreach(IDataPersistence child in dataPersistenceObjects)
        {
            child.LoadData(gameData);
        }
    }
    public void SaveData()
    {
        foreach (IDataPersistence child in dataPersistenceObjects)
        {
            child.SaveData(gameData);
        }

        dataHandler.Save(gameData);
    }

    public void ClearData()
    {
        gameData.ClearLocation();
    }

    private void OnApplicationQuit()
    {
        //For now, do NOT save automatically everytime the game quits to prevent location duplicates
        //SaveData();
    }

    private List<IDataPersistence> GetAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
