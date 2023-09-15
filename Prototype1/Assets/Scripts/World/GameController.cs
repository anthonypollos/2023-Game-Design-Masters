using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void Start()
    {
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

}
