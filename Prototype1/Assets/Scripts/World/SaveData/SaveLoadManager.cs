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

    bool isGameScene = false;
    // Start is called before the first frame update
    private void Awake()
    {
        if (SaveLoadManager.instance != null)
        {
            Debug.Log("Second instance, destroying");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("first instance, setting");
            instance = this;
            dataHandler = new FileDataHandler(Application.persistentDataPath);
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += NewScene;
            SceneManager.sceneUnloaded += LeaveScene;
        }
    }

    public void ChangeDataHandler(int slot)
    {
        PlayerPrefs.SetInt("SaveSlot", slot);
        dataHandler = new FileDataHandler(Application.persistentDataPath);
        LoadGame();
    }

    private void Start()
    {
        
    }

    [ContextMenu("Delete Save Data")]
    private void DeleteSaveData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath);
        dataHandler.Delete();
    }
    public void NewGame()
    {
        DeleteSaveData();
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
            if(isGameScene)
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
        isGameScene = false;
        if(GameController.instance !=null)
            isGameScene = !GameController.instance.GetNonGameScenes().Contains(scene.name);
        if (scene.name == "HubScene" || !isGameScene)
        {
            Debug.Log("Check to set hubReset");
            if (savedValues.currentLevel == "HubScene")
            {
                Debug.Log("set hubReset");
                savedValues.hubReset = true;
            }
        }
        if (scene.name != savedValues.currentLevel && isGameScene)
        {
            Debug.Log("Different Scene");
            Debug.Log("currentLevel was " + savedValues.currentLevel);
            Debug.Log("currentLevel now " + scene.name);
            savedValues.currentLevel = scene.name;
            savedValues.currentLevelMissionStatuses.Clear();
        }
        InitialSave();
        IEnumerable<ISaveable> saveableObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();
        this.saveableObjects = new List<ISaveable>(saveableObjects);
        LoadGame();
    }

    public SavedValues GetCopy()
    {
        return savedValues;
    }


}
