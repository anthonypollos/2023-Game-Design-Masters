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

    private void InitialLoad()
    {
        this.savedValues = dataHandler.Load();

        if (this.savedValues == null)
        {
            NewGame();
        }
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
            savedValues.currentLevel = SceneManager.GetActiveScene().name;
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
        if(savedValues == null)
        {
            InitialLoad();
        }
        IEnumerable<ISaveable> saveableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        this.saveableObjects = new List<ISaveable>(saveableObjects);
        Transform player = FindObjectOfType<PlayerHealth>().gameObject.transform;
        //Debug.Log("Current saved level: " + savedValues.currentLevel);
        if(savedValues.currentLevel != scene.name)
        {
            //Debug.Log("New level: " + scene.name);
            savedValues.currentLevel = scene.name;
            //Debug.Log("New level: " +  savedValues.currentLevel);
            savedValues.currentLevelMissionStatuses.Clear();
            savedValues.checkPointLocation = player.position;
        }
        else if(savedValues.checkPointLocation != Vector3.zero)
        {
            //Debug.Log("Setting checkpoint");
            //player.position = savedValues.checkPointLocation;
        }
        LoadGame();
    }


}
