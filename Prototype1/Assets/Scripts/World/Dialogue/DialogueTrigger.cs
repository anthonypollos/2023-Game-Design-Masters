using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using System;


public class DialogueTrigger: InteractableBehaviorTemplate
{

    [Header("Default")]
    [SerializeField] TextAsset dialogueText1;
    [SerializeField] TextAsset dialogueText2;
    [Header("Text for each level")]
    [Tooltip("Prioritized from top down")]
    [SerializeField] List<TextAssets> dialogueTexts;

    TextAsset initialDialogue;
    TextAsset talkToAgain;

    bool interacted = false;

    public void Start()
    {
        SavedValues savedValues = SaveLoadManager.instance.GetCopy();
        foreach(TextAssets asset in dialogueTexts)
        {
            bool completed;
            if(savedValues.levels.TryGetValue(asset.levelName, out completed))
            {
                if(completed)
                {
                    initialDialogue = asset.initialDialogue;
                    talkToAgain = asset.talkToAgain;
                    break;
                }

            }
        }
        if(initialDialogue == null && talkToAgain == null) 
        {
            initialDialogue = dialogueText1;
            talkToAgain = dialogueText2;
        }
    }
    public override bool Interact()
    {
        if (interacted)
        {
            if (talkToAgain != null)
                DialogueManager.instance.EnterDialogMode(talkToAgain);
            else
                Debug.Log("Second Dialogue");
        }
        else
        {
            if (initialDialogue != null)
                DialogueManager.instance.EnterDialogMode(initialDialogue);
            else
                Debug.Log("First Dialogue");
            interacted = true; 
        }
        return false;
    }
}

