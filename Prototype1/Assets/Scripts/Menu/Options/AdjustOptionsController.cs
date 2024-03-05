using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

public class AdjustOptionsController : MonoBehaviour
{
    [SerializeField] private UnityEvent leftButton, rightButton, actionButton;

    [SerializeField] private TextMeshProUGUI navTextDisplay;
    [SerializeField] private string navText = "test";

    private bool canDpad = true;

    private void OnEnable()
    {
        navTextDisplay.text = navText;
    }

    // Update is called once per frame
    void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            float dpadX = gamepad.dpad.ReadValue().x;

            if(gamepad.buttonWest.wasPressedThisFrame)
            {
                if (actionButton != null)
                    actionButton.Invoke();
            }
            else if(dpadX > 0 && canDpad)
            {
                //dpad right
                if (rightButton != null)
                    rightButton.Invoke();

                canDpad = false;
            }
            else if(dpadX < 0 && canDpad)
            {
                //dpad left
                if (leftButton != null)
                    leftButton.Invoke();

                canDpad = false;
            }
            else if (dpadX == 0)
            {
               canDpad = true;
            }
        }
    }
}
