using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum InputType {KaM = 0, Con = 1}
public class InputChecker : MonoBehaviour
{
    public static InputChecker instance;
    private InputType type;
    private InputTester inputTester;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);

        }
        else
        {
            inputTester = new InputTester();
            instance = this;
        }
    }

    private void Start()
    {
        
        type = (InputType)PlayerPrefs.GetInt("LastInput", 0);
    }
    private void OnEnable()
    {
        inputTester.Enable();
        inputTester.Main.KeyboardMouse.performed += _ => ToggleControlType(InputType.KaM);
        inputTester.Main.Controller.performed += _ => ToggleControlType(InputType.Con);
    }

    private void OnDisable()
    {
        if(inputTester!=null)
            inputTester.Disable();
    }

    public InputType GetInputType()
    {
        return type;
    }

    public bool IsController()
    {
        return type == InputType.Con;
    }


    private void ToggleControlType(InputType type)
    {
        //Debug.Log(type.ToString());
        if (type != this.type)
        {
            this.type = type;
            switch (type)
            {
                case InputType.KaM:
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    break;
                case InputType.Con:
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    break;
            }
        }
    }
}
