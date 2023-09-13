using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    MenuControls mc;
    Camera mainCamera;
    [SerializeField] bool isThirdPerson = false;
    [SerializeField] float nonCombatZoom;
    [SerializeField] float combatZoom;
    [SerializeField] float zoomTime;
    float targetZoom;
    private float velocity = 0;
    private void OnEnable()
    {
        mainCamera = Camera.main;
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
        mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, targetZoom, ref velocity, zoomTime);
    }
    public void CombatState(bool state)
    {
        if(state)
        {
            if (isThirdPerson)
                targetZoom = combatZoom;
        }
        else
        {
            if (isThirdPerson)
                targetZoom = nonCombatZoom;
        }
    }

}
