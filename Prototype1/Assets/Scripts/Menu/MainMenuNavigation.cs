using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuNavigation : MonoBehaviour
{
    MenuControls menuCont;
    MainControls mainCont;

    [SerializeField]
    private UINavManager optionsMenuNav, mainMenuNav;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        menuCont = new MenuControls();
        menuCont.Main.Enable();

        mainCont = new MainControls();
        mainCont.Main.Enable();

        menuCont.Main.Menu.performed += _ => ToggleMainMenu();
        menuCont.Main.Menu.performed += _ => ToggleOptionsMenu();
    }

    public void ToggleMainMenu()
    {
            if (optionsMenuNav.isClosed)
            {
                if (mainMenuNav.popUpActive)
                    mainMenuNav.ClosePopUpAnim("PopOut");
            }
    }

    public void ToggleOptionsMenu()
    {
            if (!optionsMenuNav.isClosed)
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
                    optionsMenuNav.CloseMainMenu("Outro");
                    mainMenuNav.OpenMainMenu("Intro");
                }
            }
    }
}
