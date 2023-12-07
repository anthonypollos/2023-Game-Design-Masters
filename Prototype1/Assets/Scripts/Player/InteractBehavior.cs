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
    Transform cam;


    private void OnEnable()
    {
        cam = Camera.main.transform;
        currentInteractables = new List<InteractableBehaviorTemplate>();
        mc = new MainControls();
        mc.Main.Interact.Enable();
        mc.Main.Interact.performed += _ => Interact();
        buttons = new List<string>();
        //Debug.Log(mc.Main.Interact.bindings.Count);
        foreach (InputBinding action in mc.Main.Interact.bindings)
        {
            buttons.Add(action.ToDisplayString());
            //Debug.Log(action.ToDisplayString());
        }
        Changed();
    }

    private void Start()
    {
        DialogueManager.instance.SetPlayerInteractionText(textBox.gameObject);
    }

    private void LateUpdate()
    {
        Vector3 pos = cam.position;
        textBox.transform.LookAt(textBox.transform.position - (pos-textBox.transform.position));

    }

    private void OnDisable()
    {
        mc.Disable(); 
    }

    private void Interact()
    {
        if (Time.timeScale != 0)
        {
            if (currentInteractables.Count > 0)
            {
                if (!currentInteractables[0].Interact())
                {
                    currentInteractables.Add(currentInteractables[0]);
                }
                else
                {
                    currentInteractables[0].gameObject.SetActive(false);
                }
                currentInteractables.RemoveAt(0);
                if(textBox.gameObject.activeInHierarchy)
                    Changed();
            }
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
            string one = "[" + buttons[(int)InputChecker.instance.GetInputType()].ToString() + "]";
            string two = currentInteractables[0].Activate();
            textBox.text = "Press " + one + " to " + two;
            textBox.gameObject.SetActive(true);
        }
        else
        {
            textBox.gameObject.SetActive(false);
        }
    }


}
