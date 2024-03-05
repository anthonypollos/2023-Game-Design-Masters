using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuNavigation : MonoBehaviour
{
    [SerializeField] private Button buttonToSelect;

    private InputChecker inputChecker;

    private bool isController = false;

    // Start is called before the first frame update
    void Start()
    {
        inputChecker = FindObjectOfType<InputChecker>();

        if (inputChecker.IsController())
            SetSelectedButton(buttonToSelect);
    }


    private void OnEnable()
    {
        // on enable for things like enabling the pause menu, level select where there is no Button to press first

        if(EventSystem.current.currentSelectedGameObject != null)
        {
            print("theres something selected");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isController != inputChecker.IsController())
        {
            isController = inputChecker.IsController();

            if (isController)
                SetSelectedButton(buttonToSelect);

            //else
                //DeselectButtons();
        }
    }

    public void SetSelectedButton(Button button)
    {
        if(inputChecker.IsController() && button != null)
        {
            buttonToSelect = button;
            buttonToSelect.Select();
        }
    }


    public void DeselectButtons()
    {
        //EventSystem.current.SetSelectedGameObject(null);
    }

}
