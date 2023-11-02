using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SaveLoadManager : MonoBehaviour
{
    private SavedValues savedValues;
    public static SaveLoadManager instance { get; private set; }

    private FileDataHandler dataHandler;

    private List<ISaveable> saveableObjects;
    // Start is called before the first frame update
    private void Awake()
    {
        if(instance!=null)
        {
            Destroy(gameObject);
        }
        instance = this;
        dataHandler = new FileDataHandler(Application.persistentDataPath);
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += NewScene;
        
    }

    private void Start()
    {
        
    }


    public void NewGame()
    {
        Debug.Log("New game");
        savedValues = new SavedValues();
    }

    public void LoadGame()
    {
        this.savedValues = dataHandler.Load();

        if(this.savedValues == null)
        {
            NewGame();
        }


        if (saveableObjects != null)
        {
            Debug.Log("Total Loadable Objects: " + saveableObjects.Count);
            foreach (ISaveable temp in saveableObjects)
            {
                temp.LoadData(savedValues);
            }
        }
        else
            Debug.Log("No Loadable Objects Found");
        
    }

    public void SaveGame()
    {
        if (saveableObjects != null)
        {
            Debug.Log("Total Saveable Objects: " + saveableObjects.Count);
            foreach (ISaveable temp in saveableObjects)
            {
                if (savedValues == null)
                {
                    Debug.LogError("savedValues = null");
                    return;
                }

                temp.SaveData(ref savedValues);
            }
        }
        else
            Debug.Log("No Saveable Objects Found");

        dataHandler.Save(savedValues);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void NewScene(Scene scene, LoadSceneMode loadMode)
    {
        
        IEnumerable<ISaveable> saveableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        this.saveableObjects = new List<ISaveable>(saveableObjects);
        LoadGame();
    }


}
