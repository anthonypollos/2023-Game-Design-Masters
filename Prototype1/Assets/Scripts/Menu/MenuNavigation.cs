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
    void Awake()
    {
        inputChecker = FindObjectOfType<InputChecker>();

        if (inputChecker.IsController())
            SetSelectedButton(buttonToSelect);
    }

    [SerializeField] private bool setOnEnable = false;

    private void OnEnable()
    {
        if(setOnEnable)
        {
            SetSelectedButton(buttonToSelect);
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

            else
                DeselectButtons();
        }
    }

    public void SetSelectedButton(Button button)
    {
        buttonToSelect = button;
        
        if (inputChecker.IsController())
            buttonToSelect.Select();
    }


    public void DeselectButtons()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

}
