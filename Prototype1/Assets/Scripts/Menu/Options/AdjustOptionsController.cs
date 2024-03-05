using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class AdjustOptionsController : MonoBehaviour
{
    [SerializeField] private UnityEvent leftButton, rightButton, actionButton;

    [SerializeField] private TextMeshProUGUI navTextDisplay;
    [SerializeField] private string navText = "test";

    private void OnEnable()
    {
        navTextDisplay.text = navText;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("joystickButton7"))
        {
            print("left");

            if(leftButton != null)
                leftButton.Invoke();
        }

        else if (Input.GetButtonDown("joystickButton5"))
        {
            print("right");

            if(rightButton != null)
                rightButton.Invoke();
        }

        else if (Input.GetButtonDown("joystickButton15"))
        {
            print("x button");

            if (actionButton != null)
                actionButton.Invoke();
        }
    }
}
