using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class InteractBehavior : MonoBehaviour
{
    List<InteractableBehaviorTemplate> currentInteractables;
    MainControls mc;
    List<string> buttons;
    [SerializeField] TextMeshProUGUI textBox;


    private void OnEnable()
    {
        mc = new MainControls();
        mc.Main.Interact.Enable();
        mc.Main.Interact.performed += _ => Interact();
        List<string> buttons = new List<string>();
        foreach (InputAction action in mc.Main.Interact.actionMap.actions)
        {
            buttons.Add(action.GetBindingDisplayString());
        }
    }

    private void OnDisable()
    {
        mc.Disable(); 
    }

    private void Interact()
    {
        if(currentInteractables.Count>0)
        {
            if(!currentInteractables[0].Interact())
            {
                currentInteractables.Add(currentInteractables[0]);
            }
            currentInteractables.RemoveAt(0);

            Changed();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Interactable"))
        {
            InteractableBehaviorTemplate temp = other.GetComponent<InteractableBehaviorTemplate>();
            if (temp != null)
                if (!currentInteractables.Contains(temp))
                    currentInteractables.Add(temp);
        }
        Changed();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            InteractableBehaviorTemplate temp = other.GetComponent<InteractableBehaviorTemplate>();
            if (temp != null)
                if (currentInteractables.Contains(temp))
                    currentInteractables.Remove(temp);
        }
        Changed();
    }

    private void Changed()
    {
        if(currentInteractables.Count>0)
        {
            textBox.text = "Press " + buttons[(int)InputChecker.instance.GetInputType()] + " to " + currentInteractables[0].Activate();
            textBox.gameObject.SetActive(true);
        }
        else
        {
            textBox.gameObject.SetActive(false);
        }
    }


}
