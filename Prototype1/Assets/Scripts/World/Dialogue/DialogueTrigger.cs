using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

public class DialogueTrigger: InteractableBehaviorTemplate
{
    [SerializeField] TextAsset dialogText;
    [SerializeField] TextAsset dialogText2;
    bool interacted = false;

    public override bool Interact()
    {
        if (interacted)
        {
            if (dialogText2 != null)
                DialogueManager.instance.EnterDialogMode(dialogText2);
            else
                Debug.Log("Second Dialogue");
        }
        else
        {
            if (dialogText != null)
                DialogueManager.instance.EnterDialogMode(dialogText);
            else
                Debug.Log("First Dialogue");
        }
        return false;
    }
}

