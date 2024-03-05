using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsButtonAnim : MonoBehaviour
{
    enum PanelType {cycle, slider, button };

    [SerializeField] private PanelType panelType;

    private Animator anim;
    private InputChecker inputChecker;
    private bool isController = false;

    private void OnEnable()
    {
        anim = GetComponent<Animator>();
        inputChecker = FindObjectOfType<InputChecker>();

        if (inputChecker.IsController())
            anim.SetBool("Controller", true);

        switch (panelType)
        {
            case PanelType.cycle:
                anim.SetBool("Cycle", true);
                break;
            case PanelType.slider:
                anim.SetBool("Slider", true);
                break;
            case PanelType.button:
                anim.SetBool("Button", true);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isController != inputChecker.IsController())
        {
            isController = inputChecker.IsController();

            if (isController)
                anim.SetBool("Controller", true);

            else
                anim.SetBool("Controller", false);
        }
    }
}
