using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UINavManager : MonoBehaviour
{
    [Tooltip("Is this menu in a sub-menu?")]
    public bool subMenuActive;
    [Tooltip("Is this menu displaying a pop-up?")]
    public bool popUpActive;
    [Tooltip("Is this menu disabled?")]
    public bool isClosed = true;

    [Tooltip("This prefab's Main Menu; Should be immediately child of this object")]
    [SerializeField] private GameObject mainMenu;

    [Tooltip("Animator Component; Should be on the mainMenu GameObject")]
    [SerializeField] private Animator anim;

    [Tooltip("Is this menu in the Main Menu scene?")]
    private bool isMainMenu = false;

    private void Awake()
    {
        isMainMenu = (SceneManager.GetActiveScene().name == "MainMenu_New");
    }

    /// <summary>
    /// opens desired menu, sets timeScale to 0 and unlocks cursor
    /// </summary>
    public void OpenMainMenu()
    {
        isClosed = false;
        subMenuActive = false;
        popUpActive = false;

        mainMenu.SetActive(true);

        if(!isMainMenu)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
    }

    public void OpenMainMenu(string animTrigger)
    {
        isClosed = false;
        subMenuActive = false;
        popUpActive = false;

        anim.SetTrigger(animTrigger);

        if (!isMainMenu)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
    }

    public void OpenDeathMenu()
    {
        isClosed = false;
        subMenuActive = false;
        popUpActive = false;

        mainMenu.SetActive(true);

        if (!isMainMenu)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    /// <summary>
    /// closes desired menu/returns to gameplay, sets timeScale back to 1 and locks cursor
    /// </summary>
    public void CloseMainMenu()
    {
        isClosed = true;
        subMenuActive = false;
        popUpActive = false;

        mainMenu.SetActive(false);

        if (!isMainMenu)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 1;
        }
    }

    public void CloseMainMenu(string animTrigger)
    {
        anim.SetTrigger(animTrigger);

        isClosed = true;
        subMenuActive = false;
        popUpActive = false;
    }

    public void CloseAnim()
    {
        if (!isMainMenu)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 1;
        }
    }

    public void DisableMain()
    {
        mainMenu.SetActive(false);
    }

    /// <summary>
    /// Closes any sub-menus or pop-ups & returns to main menu
    /// </summary>
    /// <param name="animTrigger">anim trigger to return to main menu; likely called "Main"</param>
    public void ReturnToMain(string animTrigger)
    {
        subMenuActive = false;
        popUpActive = false;

        anim.SetTrigger(animTrigger);
    }

    /// <summary>
    /// Opens sub-menu specified by animTrigger variable
    /// </summary>
    /// <param name="animTrigger">anim Trigger related to desired sub-menu</param>
    public void OpenSubMenu(string animTrigger)
    {
        subMenuActive = true;

        anim.SetTrigger(animTrigger);
    }

    /// <summary>
    /// Closes active sub-menu, returns to main menu
    /// </summary>
    /// <param name="animTrigger">anim trigger to return to main menu; likely called "Main"</param>
    public void CloseSubMenu(string animTrigger)
    {
        subMenuActive = false;

        anim.SetTrigger(animTrigger);
    }

    /// <summary>
    /// Opens pop-up window
    /// </summary>
    /// <param name="animTrigger">anim trigger to open pop-up window; likely called "PopUp"</param>
    public void OpenPopUp(string animTrigger)
    {
        popUpActive = true;

        anim.SetTrigger(animTrigger);
    }

    /// <summary>
    /// Closes active pop-up window, returns to main menu
    /// </summary>
    /// <param name="animTrigger">anim trigger to return to main menu; likely called "Main"</param>
    public void ClosePopUp(string animTrigger)
    {
        popUpActive = false;

        if(anim != null)
            anim.SetTrigger(animTrigger);
    }
}
