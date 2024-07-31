using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsButtonAnim : MonoBehaviour
{
    enum PanelType {cycle, slider, button, sliderSmall, colorPick, rebind };

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
            case PanelType.sliderSmall:
                anim.SetBool("SliderSmall", true);
                break;
            case PanelType.colorPick:
                anim.SetBool("ColorPick", true);
                break;
            case PanelType.rebind:
                anim.SetBool("Rebind", true);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*if (isController != inputChecker.IsController())
        {
            isController = inputChecker.IsController();

            if (isController)
                anim.SetBool("Controller", true);

            else
                anim.SetBool("Controller", false);
        }*/
    }
}
