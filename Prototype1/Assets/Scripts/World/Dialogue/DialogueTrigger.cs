using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

public class DialogueTrigger: InteractableBehaviorTemplate
{
    [SerializeField] TextAsset dialogText;

    public override bool Interact()
    {
        DialogueManager.instance.EnterDialogMode(dialogText);
        return false;
    }
}

