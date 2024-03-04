using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuNavigation : MonoBehaviour
{
    [SerializeField] private GameObject firstButton;
    [SerializeField] private EventSystem eventSystem;

    private InputChecker inputChecker;

    private bool isController = false;

    // Start is called before the first frame update
    void Start()
    {
        inputChecker = FindObjectOfType<InputChecker>();

        if (inputChecker.IsController())
            SetSelectedButton(firstButton);
    }

    // Update is called once per frame
    void Update()
    {
        if(isController != inputChecker.IsController())
        {
            isController = inputChecker.IsController();

            if(isController)
                SetSelectedButton(firstButton);

            else
                DeselectButtons();
        }
    }


    public void SetSelectedButton(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(button);
    }


    public void DeselectButtons()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

}
