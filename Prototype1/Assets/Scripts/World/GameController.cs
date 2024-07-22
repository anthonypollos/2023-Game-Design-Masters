using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FMODUnity;

public class GameController : MonoBehaviour
{

    MenuControls mc;
    MainControls mainCont;
    List<Camera> mainCameras;
    [SerializeField] bool isThirdPerson = false;
    [SerializeField] [Tooltip("First value = orhographic zoom, Second value = vertical FOV")] float[] nonCombatZoom = { 5, 15 };
    [SerializeField] [Tooltip("First value = orhographic zoom, Second value = vertical FOV")] float[] combatZoom = { 10, 40 };
    [SerializeField] float zoomTime = 1;
    float[] targetZoom;
    private float velocity = 0;
    [HideInInspector]
    public bool toggleLasso = false;
    private TextMeshProUGUI text;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject[] menus;
    [SerializeField] GameObject deathMenu;
    //[SerializeField] GameObject journalMenu;
    //[SerializeField] Button topButtonPause;   //not currently needed with no controller support
    //[SerializeField] Button topButtonDead;
    [SerializeField] List<string> nonGameScenes;
    [SerializeField] List<string> cutSceneScenes;
    [SerializeField] List<string> levelTransitionScenes;

    [Header("UI Navigvation")]
    //[SerializeField] private LevelSelectManager levelSelectManager;

    [SerializeField] private UINavManager pauseMenuNav;
    [SerializeField] private UINavManager journalMenuNav,levelSelectMenuNav, dialogueMenuNav,
                                           optionsMenuNav, collectMenuNav, deathMenuNav;

    bool inCombat = true;

    bool paused;
    bool journal;


    static GameObject player;

    public static GameController instance;

    //private OutlineToggle outlineManager;
    [Header("FMOD")]
    public GameObject AManager;
    public GameObject CombatMusicManager;
    public SavedValues savedValuesInstance;

    private void Awake()
    {
        //outlineManager = FindObjectOfType<OutlineToggle>();
        paused = false;
        //journalOpen = false;
        Cursor.lockState = CursorLockMode.Confined;
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
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

    public List<string> GetNonGameScenes()
    {
        return nonGameScenes;
    }

    public List<string> GetCutSceneScenes()
    {
        return cutSceneScenes;
    }

    public List<string> GetLevelTransitionScenes()
    {
        return levelTransitionScenes;
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

        mainCont = new MainControls();
        mainCont.Main.Enable();

        mc.Main.Menu.performed += _ => TogglePauseMenu();
        mc.Main.Menu.performed += _ => ToggleOptionsMenu();
        mc.Main.Restart.performed += _ => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        mc.Main.ToggleLasso.performed += _ => ToggleLasso();
        mc.Main.Journal.performed += _ => ToggleJournal();
        mc.Main.Menu.performed += _ => CloseJournal();
        if (levelSelectMenuNav != null)
            mc.Main.Menu.performed += _ => ToggleLevelSelect();
        if (collectMenuNav != null)
        {
            mc.Main.Menu.performed += _ => ToggleCollectibleView();
            mainCont.Main.Interact.performed += _ => ToggleCollectibleView();
        }

        //CombatState(true);
    }

    private void OnDisable()
    {
        CombatState(false);
        mc.Disable();
    }

    #region UI Navigation

    public void TogglePauseMenu()
    {
        if (!nonGameScenes.Contains(SceneManager.GetActiveScene().name) &&
            !cutSceneScenes.Contains(SceneManager.GetActiveScene().name) &&
            !DeveloperConsole.instance.consoleUI.activeInHierarchy)
        {
            if (!pauseMenuNav.isClosed)
            {
                // if pause menu pop-up is active, close pop-up
                if (pauseMenuNav.popUpActive)
                    pauseMenuNav.ClosePopUp("Main");
                // if pause menu is open to a sub-menu (ie options), close sub-menu and return to main pause menu
                else if (pauseMenuNav.subMenuActive)
                    pauseMenuNav.CloseSubMenu("Main");
                // if pause menu is on main pause menu, close pause menu
                else
                    pauseMenuNav.CloseMainMenu();

                //paused = false;
                //Cursor.lockState = CursorLockMode.Confined;                     // previous/commented code moved to UINavManager script
                //pauseMenu.SetActive(false);
                //foreach (GameObject menu in menus)
                //    menu.SetActive(false);
                //Time.timeScale = 1;
            }
            // if pause menu/other UI not open, open pause menu
            else if (Time.timeScale != 0) //|| !dialogueMenuNav.isClosed)
            {
                pauseMenuNav.OpenMainMenu();
                //paused = true;
                //Cursor.lockState = CursorLockMode.None;
                //pauseMenu.SetActive(true);
                ////topButtonPause.Select();
                //print("help");
                //Time.timeScale = 0;
            }
        }
    }

    // Options Nav using Menu Key; Menu Key will only close options (sub)menus and pop-ups
    public void ToggleOptionsMenu()
    {
        // if not on main menu
        if (!nonGameScenes.Contains(SceneManager.GetActiveScene().name) &&
            !cutSceneScenes.Contains(SceneManager.GetActiveScene().name) &&
            !DeveloperConsole.instance.consoleUI.activeInHierarchy)
        {
            if(!optionsMenuNav.isClosed)
            {
                // if options menu is on a pop-up within a sub-menu, close pop-up and return to sub-menu
                if (optionsMenuNav.subMenuActive && optionsMenuNav.popUpActive)
                    optionsMenuNav.ClosePopUp("SubMenu");
                // if options menu open to sub-menu, close sub-menu and return to main
                else if (optionsMenuNav.subMenuActive)
                    optionsMenuNav.CloseSubMenu("Main");
                // if options menu open to pop-up, close pop-up
                else if (optionsMenuNav.popUpActive)
                    optionsMenuNav.ClosePopUp("Main");
                else
                {
                    optionsMenuNav.CloseMainMenu();
                    pauseMenuNav.OpenMainMenu();
                }
            }
        }
        // if on main menu; maybe TEMP, use less redundant way to do this
        else if (nonGameScenes.Contains(SceneManager.GetActiveScene().name) &&
                !cutSceneScenes.Contains(SceneManager.GetActiveScene().name))
        {
            if (!optionsMenuNav.isClosed)
            {
                // if options menu is on a pop-up within a sub-menu
                if (optionsMenuNav.subMenuActive && optionsMenuNav.popUpActive)
                    optionsMenuNav.ClosePopUp("SubMenu");
                // if options menu open to sub-menu, close sub-menu and return to main
                else if (optionsMenuNav.subMenuActive)
                    optionsMenuNav.CloseSubMenu("Main");
                // if options menu open to pop-up, close pop-up
                else if (optionsMenuNav.popUpActive)
                    optionsMenuNav.ClosePopUp("Main");
                else
                {
                    optionsMenuNav.CloseMainMenu();
                    // open main menu again
                }
            }
        }
    }

    // Journal Nav using Journal Key
    public void ToggleJournal()
    {
        if  (!nonGameScenes.Contains(SceneManager.GetActiveScene().name) &&
            !cutSceneScenes.Contains(SceneManager.GetActiveScene().name) &&
            !DeveloperConsole.instance.consoleUI.activeInHierarchy)
        {
            if(Time.timeScale != 0)
            {
                // if journal/other UI not open, open to main contents page
                if (journalMenuNav.isClosed)
                {
                    savedValuesInstance = SaveLoadManager.instance.GetCopy();

                    journalMenuNav.OpenMainMenu();
                }
            }
            // if journal is on main contents page, close journal
            else if (!journalMenuNav.isClosed && !journalMenuNav.subMenuActive)
            {
                journalMenuNav.CloseMainMenu();
            }
            // if journal is on sub-menu, return to main contents page
            else if (journalMenuNav.subMenuActive)
                journalMenuNav.ReturnToMain("Main");
        }
    }

    // Journal Nav using Menu Key; Menu Key will only close journal (sub)menus
    public void CloseJournal()
    {
        if (!nonGameScenes.Contains(SceneManager.GetActiveScene().name) &&
            !cutSceneScenes.Contains(SceneManager.GetActiveScene().name) &&
            !DeveloperConsole.instance.consoleUI.activeInHierarchy)
        {
            if(!journalMenuNav.isClosed)
            {
                // if journal is on main contents page, close journal
                if (!journalMenuNav.subMenuActive)
                    journalMenuNav.CloseMainMenu();
                // if journal is on sub-menu, return to main contents page
                else if (journalMenuNav.subMenuActive)
                    journalMenuNav.ReturnToMain("Main");
            }
        }
    }

    // Level Select Navigation; Menu Key will only close level select (sub)menus and pop-ups
    public void ToggleLevelSelect()
    {
        if (!nonGameScenes.Contains(SceneManager.GetActiveScene().name) &&
            !cutSceneScenes.Contains(SceneManager.GetActiveScene().name) &&
            !DeveloperConsole.instance.consoleUI.activeInHierarchy)
        {
            if(!levelSelectMenuNav.isClosed)
            {
                // if level select extras menu or confirmation pop-up active, return to main level select menu
                if (levelSelectMenuNav.subMenuActive || levelSelectMenuNav.popUpActive)
                    levelSelectMenuNav.ReturnToMain("Main");
                // else if not ^, close level select menu
                else
                    levelSelectMenuNav.CloseMainMenu();
            }
        }
    }

    public void ToggleCollectibleView()
    {
        if (!nonGameScenes.Contains(SceneManager.GetActiveScene().name) &&
            !cutSceneScenes.Contains(SceneManager.GetActiveScene().name) &&
            !DeveloperConsole.instance.consoleUI.activeInHierarchy)
        {
            if (!collectMenuNav.isClosed)
                collectMenuNav.CloseMainMenu("NextState");
        }
    }

    public void Lose()
    {
        Cursor.lockState = CursorLockMode.None;
        deathMenu.SetActive(true);
        //topButtonDead.Select();
        Time.timeScale = 0;
    }

    #endregion

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

    public void StartBossFight(EventReference bossTheme)
    {
        //Debug.Log("Changing boss fight music");
        StudioEventEmitter emitter = CombatMusicManager.GetComponent<StudioEventEmitter>();
        emitter.Stop();
        emitter.ChangeEvent(bossTheme);
        emitter.Play();
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
                ObjectiveManager.Instance.ToggleHud(false);
            }
            else
            {
                targetZoom = nonCombatZoom;
                AManager.GetComponent<FMODUnity.StudioEventEmitter>().Play();
                if (CombatMusicManager != null) CombatMusicManager.GetComponent<FMODUnity.StudioEventEmitter>().Stop();
                ObjectiveManager.Instance.ToggleHud(true);
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

        GameObject temp = go.FindChildObjectWithTag("Tendriled");
        go.SetActive(true);
        if (temp != null)
        {
            temp.SetActive(false);
        }
        //Add the respawned object to the outline manager
        foreach (Transform child in go.transform)
        {
            //Check the children of the respawned object. Find whatever child has the outline and add it
            if (child.GetComponent<Outline>() != null)
            {
                //if(outlineManager != null)
                    //outlineManager.AddOutline(child.gameObject);
                //de-activate the outline
                child.GetComponent<Outline>().enabled = false;
            }
        }
            
    }

    private IEnumerator DelayedRespawnFrozen(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject temp = go.FindChildObjectWithTag("Tendriled");
        go.SetActive(true);
        if(temp!= null)
        {
            temp.SetActive(false);
        }
        //Add the respawned object to the outline manager
        foreach (Transform child in go.transform)
        {
            //Check the children of the respawned object. Find whatever child has the outline and add it
            if (child.GetComponent<Outline>() != null)
            {
                //if (outlineManager != null)
                    //outlineManager.AddOutline(child.gameObject);
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
