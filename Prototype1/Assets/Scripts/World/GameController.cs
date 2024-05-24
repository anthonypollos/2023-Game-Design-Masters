using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    MenuControls mc;
    List<Camera> mainCameras;
    [SerializeField] bool isThirdPerson = false;
    [SerializeField][Tooltip("First value = orhographic zoom, Second value = vertical FOV")] float[] nonCombatZoom = { 5, 15 };
    [SerializeField][Tooltip("First value = orhographic zoom, Second value = vertical FOV")] float[] combatZoom = { 10, 40 };
    [SerializeField] float zoomTime = 1;
    float[] targetZoom;
    private float velocity = 0;
    [HideInInspector]
    public bool toggleLasso = false;
    private TextMeshProUGUI text;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject[] menus;
    [SerializeField] GameObject deathMenu;
    [SerializeField] GameObject journalMenu;
    [SerializeField] Button topButtonPause;
    [SerializeField] Button topButtonDead;
    [SerializeField] List<string> nonGameScenes;
    bool inCombat = true;

    bool paused;
    bool journalOpen;

    static GameObject player;

    public static GameController instance;

    private OutlineToggle outlineManager;
    public GameObject AManager;
    public GameObject CombatMusicManager;
    public SavedValues savedValuesInstance;

    private void Awake()
    {
        outlineManager = FindObjectOfType<OutlineToggle>();
        paused = false;
        journalOpen = false;
        Cursor.lockState = CursorLockMode.Confined;
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            player = null;
        }
    }

    public static Transform GetPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        //Debug.Log(player.transform.position);
        return player.transform;

    }


    private void Start()
    {
        //CombatMusicManager.GetComponent<FMODUnity.StudioEventEmitter>().Stop();
        Time.timeScale = 1;
        mainCameras = new List<Camera>();
        GameObject[] temp = GameObject.FindGameObjectsWithTag("MainCamera");
        foreach (GameObject go in temp) {
            mainCameras.Add(go.GetComponent<Camera>());
        }
        if (CombatMusicManager != null)
        CombatMusicManager.GetComponent<FMODUnity.StudioEventEmitter>().Stop();
        CombatState(false);
    }
    private void OnEnable()
    {
        mc = new MenuControls();
        mc.Main.Enable();
        mc.Main.Menu.performed += _ => TogglePauseMenu();
        mc.Main.Restart.performed += _ => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        mc.Main.ToggleLasso.performed += _ => ToggleLasso();
        mc.Main.Journal.performed += _ => ToggleJournal();

        //CombatState(true);
    }

    private void OnDisable()
    {
        CombatState(false);
        mc.Disable();
    }

    public void TogglePauseMenu()
    {
        if (!nonGameScenes.Contains(SceneManager.GetActiveScene().name) && !DeveloperConsole.instance.consoleUI.activeInHierarchy)
        {
            if(journalOpen)
            {
                journalOpen = false;
                Cursor.lockState = CursorLockMode.Confined;
                journalMenu.SetActive(false);
                Time.timeScale = 1;
            }
            if (paused)
            {
                paused = false;
                Cursor.lockState = CursorLockMode.Confined;
                pauseMenu.SetActive(false);
                foreach (GameObject menu in menus)
                    menu.SetActive(false);
                Time.timeScale = 1;
            }
            else if (Time.timeScale != 0)
            {
                paused = true;
                Cursor.lockState = CursorLockMode.None;
                pauseMenu.SetActive(true);
                //topButtonPause.Select();
                Time.timeScale = 0;
            }
        }
    }

    public void ToggleJournal()
    {
        if (!nonGameScenes.Contains(SceneManager.GetActiveScene().name) && !DeveloperConsole.instance.consoleUI.activeInHierarchy)
        {
            if (journalOpen)
            {
                journalOpen = false;
                Cursor.lockState = CursorLockMode.Confined;
                journalMenu.SetActive(false);
                Time.timeScale = 1;
            }
            else if (Time.timeScale != 0)
            {
                savedValuesInstance = SaveLoadManager.instance.GetCopy();
                journalOpen = true;
                Cursor.lockState = CursorLockMode.None;
                journalMenu.SetActive(true);
                //topButtonPause.Select();
                Time.timeScale = 0;
            }
        }
    }

    public void Lose()
    {
        Cursor.lockState = CursorLockMode.None;
        deathMenu.SetActive(true);
        topButtonDead.Select();
        Time.timeScale = 0;
    }

    public void SetScene(string scene)
    {
        Time.timeScale = 1;
        if (scene.Equals("Reset"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        else
            SceneManager.LoadScene(scene);

    }

    private void Update()
    {
        if (isThirdPerson)
        {
            foreach (Camera cam in mainCameras)
            {
                if (cam.orthographic)
                {
                    cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetZoom[0], ref velocity, zoomTime);
                }
                else //if vertical
                {
                    cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, targetZoom[1], ref velocity, zoomTime);
                }

            }
        }
    }
    public void CombatState(bool state)
    {
        if (state == inCombat)
            return;
        inCombat = state;
        if(isThirdPerson)
            if(state)
            {
                targetZoom = combatZoom;
                if (CombatMusicManager != null) CombatMusicManager.GetComponent<FMODUnity.StudioEventEmitter>().Play();
                AManager.GetComponent<FMODUnity.StudioEventEmitter>().Stop();
            }
            else
            {
                targetZoom = nonCombatZoom;
                AManager.GetComponent<FMODUnity.StudioEventEmitter>().Play();
                if (CombatMusicManager != null) CombatMusicManager.GetComponent<FMODUnity.StudioEventEmitter>().Stop();
            }
    }

    private void ToggleLasso()
    {
        toggleLasso = !toggleLasso;
    }

    public void ButtonSelect(Button select)
    {
        select.Select();
    }


    public void Respawn(GameObject go, float delay, Vector3 pos, Quaternion rot)
    {
        if (!go.gameObject.scene.isLoaded)
            return;
        GameObject temp = Instantiate(go, pos, rot);
        Destroy(go);
        /*MeshRenderer mr = temp.GetComponentInChildren<MeshRenderer>();
        List<Material> mats = new List<Material>(mr.materials);
        Debug.Log("Materials " + mats.Count);
        int i = 0;
        foreach (Material mat in mats)
        {
            Debug.Log("material " + i);
            Debug.Log(mat);
            i++;
        }
        mats.RemoveAll(null);
        mr.SetMaterials(mats);
        */
        StartCoroutine(DelayedRespawn(temp, delay));
    }

    public void Respawn(GameObject go, float delay, Vector3 pos, Quaternion rot, bool frozenstatus)
    {
        if (!go.gameObject.scene.isLoaded)
            return;
        GameObject temp = Instantiate(go, pos, rot);
        Destroy(go);
        /*
        MeshRenderer mr = temp.GetComponentInChildren<MeshRenderer>();
        List<Material> mats = new List<Material>(mr.materials);
        Debug.Log("Materials " + mats.Count);
        int i = 0;
        foreach(Material mat in mats)
        {
            Debug.Log("material " + i);
            Debug.Log(mat);
            i++;
        }
        mats.RemoveAll(null);
        mr.SetMaterials(mats);
        */
        //If the gameobject we're respawning has "_frozenBeforeTendril" set to true, we run delayedrespawnfrozen so that the new one will ALSO be frozen on spawn
        if (frozenstatus) StartCoroutine(DelayedRespawnFrozen(temp, delay));
        //Otherwise, spawn as normal
        else StartCoroutine(DelayedRespawn(temp, delay));
    }


    private IEnumerator DelayedRespawn(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        go.SetActive(true);
        //Add the respawned object to the outline manager
        foreach (Transform child in go.transform)
        {
            //Check the children of the respawned object. Find whatever child has the outline and add it
            if (child.GetComponent<Outline>() != null)
            {
                if(outlineManager != null)
                    outlineManager.AddOutline(child.gameObject);
                //de-activate the outline
                child.GetComponent<Outline>().enabled = false;
            }
        }
            
    }

    private IEnumerator DelayedRespawnFrozen(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);

        go.SetActive(true);
        //Add the respawned object to the outline manager
        foreach (Transform child in go.transform)
        {
            //Check the children of the respawned object. Find whatever child has the outline and add it
            if (child.GetComponent<Outline>() != null)
            {
                if (outlineManager != null)
                    outlineManager.AddOutline(child.gameObject);
                //de-activate the outline
                child.GetComponent<Outline>().enabled = false;
            }
        }
        //Set the object's rbconstraints to "None"
        go.GetComponent<GenericItem>().SetDefaultConstraints();
        //Freeze the object
        go.GetComponent<GenericItem>().Freeze();
    }

}
