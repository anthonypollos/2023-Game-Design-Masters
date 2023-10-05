using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] GameObject deathMenu;
    [SerializeField] Button topButtonPause;
    [SerializeField] Button topButtonDead;
    [SerializeField] List<string> nonGameScenes;

    static GameObject player;

    public static GameController instance;

    private void Awake()
    {
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
        return player.transform;

    }


    private void Start()
    {
        Time.timeScale = 1;
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = "Instant Lasso: " + toggleLasso;
        mainCameras = new List<Camera>();
        GameObject[] temp = GameObject.FindGameObjectsWithTag("MainCamera");
        foreach (GameObject go in temp) {
            mainCameras.Add(go.GetComponent<Camera>());
        }
    }
    private void OnEnable()
    {
        mc = new MenuControls();
        mc.Main.Enable();
        mc.Main.Menu.performed += _ => TogglePauseMenu();
        mc.Main.Restart.performed += _ => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        mc.Main.ToggleLasso.performed += _ => ToggleLasso();
        CombatState(false);
    }

    private void OnDisable()
    {
        mc.Disable();
    }

    private void TogglePauseMenu()
    {
        if (!nonGameScenes.Contains(SceneManager.GetActiveScene().name))
        {
            if (pauseMenu.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.Confined;
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
            }
            else if (Time.timeScale != 0)
            {
                Cursor.lockState = CursorLockMode.None;
                pauseMenu.SetActive(true);
                topButtonPause.Select();
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
        if(isThirdPerson)
            if(state)
            {
                targetZoom = combatZoom;
            }
            else
            {
                targetZoom = nonCombatZoom;
            }
    }

    private void ToggleLasso()
    {
        toggleLasso = !toggleLasso;
        text.text = "Instant Lasso: " + toggleLasso;
    }

}
