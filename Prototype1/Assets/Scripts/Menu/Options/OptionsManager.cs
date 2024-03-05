using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mouseNavText, controllerNavText;
    [SerializeField] private Button[] panels;

    private InputChecker inputChecker;
    private bool isController = false;

    // Start is called before the first frame update
    void Start()
    {
        inputChecker = FindObjectOfType<InputChecker>();

        if (inputChecker.IsController())
            SetController();
        else
            SetMouse();
    }

    // Update is called once per frame
    void Update()
    {
        if (isController != inputChecker.IsController())
        {
            isController = inputChecker.IsController();

            if (isController)
                SetController();

            else
                SetMouse();
        }
    }

    private void SetMouse()
    {
        mouseNavText.gameObject.SetActive(true);
        controllerNavText.gameObject.SetActive(false);

        foreach (Button panel in panels)
        {
            Navigation navigation = panel.navigation;
            navigation.mode = Navigation.Mode.None;
            panel.navigation = navigation;
        }
    }

    private void SetController()
    {
        mouseNavText.gameObject.SetActive(false);
        controllerNavText.gameObject.SetActive(true);

        foreach (Button panel in panels)
        {
            Navigation navigation = panel.navigation;
            navigation.mode = Navigation.Mode.Explicit;
            panel.navigation = navigation;
        }
    }
}
