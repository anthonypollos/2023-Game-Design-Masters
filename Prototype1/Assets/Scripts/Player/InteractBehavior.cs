using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class InteractBehavior : MonoBehaviour
{
    List<InteractableBehaviorTemplate> currentInteractables;
    InteractableBehaviorTemplate previous;
    MainControls mc;
    List<string> buttons;
    [SerializeField] TextMeshProUGUI interactKey, interactText;
    [SerializeField] GameObject interactPromptUI;
    Transform cam;


    private void OnEnable()
    {
        cam = Camera.main.transform;
        currentInteractables = new List<InteractableBehaviorTemplate>();
        mc = ControlsContainer.instance.mainControls;
        mc.Main.Interact.performed+= Interactions;
        buttons = new List<string>();
        //Debug.Log(mc.Main.Interact.bindings.Count);
        foreach (InputBinding action in mc.Main.Interact.bindings)
        {
            buttons.Add(action.ToDisplayString().ToUpper().TranslateToSprite());
            //Debug.Log(action.ToDisplayString());
        }
        Changed();
    }

    private void Start()
    {
        if(DialogueManager.instance!=null)
            DialogueManager.instance.SetPlayerInteraction(this);
        if(NoteManager.instance!=null)
            NoteManager.instance.SetPlayerInteraction(this);
    }

    private void LateUpdate()
    {
        //Vector3 pos = cam.position;
        //textBox.transform.LookAt(textBox.transform.position - (pos-textBox.transform.position));

    }

    private void OnDisable()
    {
        mc.Main.Interact.performed -= Interactions;
    }

    private void Interactions(InputAction.CallbackContext ctx)
    {
        //Debug.Log("interact");
        if (ctx.performed)
            Interact();
    }

    private void Interact()
    {
        //Debug.Log("Interact");
        if (Time.timeScale != 0)
        {
            if (currentInteractables.Count > 0)
            {
                if(previous == currentInteractables[0])
                {
                    previous = null;
                    return;
                }
                if (!currentInteractables[0].Interact())
                {
                    currentInteractables.Add(currentInteractables[0]);
                }
                else
                {
                    currentInteractables[0].gameObject.SetActive(false);
                }
                currentInteractables.RemoveAt(0);
                if(interactText!=null && interactPromptUI.activeInHierarchy)
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
                {
                    currentInteractables.Add(temp);
                }
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
                {
                    if(previous == temp)
                    {
                        previous = null;
                    }
                    currentInteractables.Remove(temp);
                }
        }
        Changed();
    }

    public void Toggle()
    {
        if(interactText != null && interactText.gameObject.activeInHierarchy)
        {
            interactPromptUI.SetActive(false);
        }
        else
        {
            Changed();
        }
    }

    private void Changed()
    {
        if(currentInteractables.Count>0)
        {
            string one = buttons[0]; //+ "/" + buttons[1];

            string two = currentInteractables[0].Activate();
            if (interactText != null)
            {
                interactKey.text = one;
                interactText.text = two;
                interactPromptUI.SetActive(true);
            }
        }
        else
        {
            if(interactText != null)
                interactPromptUI.SetActive(false);
        }
    }


}
