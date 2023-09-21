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
    public bool toggleLasso = false;
    private TextMeshProUGUI text;


    private void Start()
    {
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
        mc.Main.Menu.performed += _ => SceneManager.LoadScene("MainMenu");
        mc.Main.Restart.performed += _ => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        mc.Main.ToggleLasso.performed += _ => ToggleLasso();
        CombatState(false);
    }

    private void OnDisable()
    {
        mc.Disable();
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
