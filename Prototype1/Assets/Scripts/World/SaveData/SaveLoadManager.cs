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
        SceneManager.sceneUnloaded += LeaveScene;
    }

    private void Start()
    {
        
    }


    public void NewGame()
    {
        Debug.Log("New game");
        savedValues = new SavedValues();
        SaveGame();
        LoadGame();
    }

    public void LoadGame()
    {
        if (dataHandler != null)
        {
            instance.savedValues = dataHandler.Load();

            if (instance.savedValues == null)
            {
                NewGame();
            }


            if (saveableObjects != null)
            {
                //Debug.Log("Total Loadable Objects: " + saveableObjects.Count);
                foreach (ISaveable temp in saveableObjects)
                {
                    temp.LoadData(savedValues);
                }
            }
            else
            {
                //Debug.Log("No Loadable Objects Found");
            }
        }
        
    }

    private void InitialLoad()
    {
        if (dataHandler != null)
        {


            instance.savedValues = dataHandler.Load();

            if (instance.savedValues == null)
            {
                NewGame();
            }
        }
    }

    private void InitialSave()
    {
        dataHandler.Save(savedValues);
    }

    private void LeaveScene(Scene scene)
    {
        SaveGame();
    }

    public void SaveGame()
    {
        Debug.Log("Saving");
        if (saveableObjects != null)
        {
            //Debug.Log("Total Saveable Objects: " + saveableObjects.Count);
            foreach (ISaveable temp in saveableObjects)
            {
                if (savedValues == null)
                {
                    //Debug.LogError("savedValues = null");
                    return;
                }

                temp.SaveData(ref savedValues);
            }
            savedValues.currentLevel = SceneManager.GetActiveScene().name;
        }
        else
        {
            //Debug.Log("No Saveable Objects Found");
        }
            

        dataHandler.Save(savedValues);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void NewScene(Scene scene, LoadSceneMode loadMode)
    {
        InitialLoad();
        if(scene.name != savedValues.currentLevel)
        {
            Debug.Log("Different Scene");
            savedValues.currentLevel = scene.name;
            savedValues.currentLevelMissionStatuses.Clear();
        }
        InitialSave();
        IEnumerable<ISaveable> saveableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        this.saveableObjects = new List<ISaveable>(saveableObjects);
        LoadGame();
    }


}
