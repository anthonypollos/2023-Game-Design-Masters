using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TextAssets
{
    public string levelName;
    public TextAsset initialDialogue;
    public TextAsset talkToAgain;
}
public class NPCMission : InteractableBehaviorTemplate
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

    private TalkToMission mission;

    public void Start()
    {
        SavedValues savedValues = SaveLoadManager.instance.GetCopy();
        foreach (TextAssets asset in dialogueTexts)
        {
            bool completed;
            if (savedValues.levels.TryGetValue(asset.levelName, out completed))
            {
                if (completed)
                {
                    initialDialogue = asset.initialDialogue;
                    talkToAgain = asset.talkToAgain;
                    break;
                }

            }
        }
        if (initialDialogue == null && talkToAgain == null)
        {
            initialDialogue = dialogueText1;
            talkToAgain = dialogueText2;
        }
    }
    public void SetMission(TalkToMission mission)
    {
        this.mission = mission;
    }

    public void TalkedTo()
    {
        interacted = true;
    }

    public override bool Interact()
    {
        if (!interacted)
        {
            if (initialDialogue != null)
                DialogueManager.instance.EnterDialogMode(initialDialogue);
            else
                Debug.Log("First Dialogue");
            interacted = true; 
            mission.TalkedTo();
        }
        else
        {
            if (talkToAgain != null)
                DialogueManager.instance.EnterDialogMode(talkToAgain);
            else
                Debug.Log("Second Dialogue");
        }
        return false;
    }
}
