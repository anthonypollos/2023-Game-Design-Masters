using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    MenuControls mc;
    private void OnEnable()
    {
        mc = new MenuControls();
        mc.Main.Enable();
        mc.Main.Menu.performed += _ => SceneManager.LoadScene("MainMenu");
        mc.Main.Restart.performed += _ => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDisable()
    {
        mc.Disable();
    }
}
