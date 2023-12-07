using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMission : InteractableBehaviorTemplate
{
    [SerializeField] TextAsset dialogText1;
    [SerializeField] TextAsset dialogText2;
    bool interacted = false;

    private TalkToMission mission;

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
            if (dialogText1 != null)
            {
                DialogueManager.instance.EnterDialogMode(dialogText1);
            }
            else
            {
                Debug.Log("Mission talk");
            }
            mission.TalkedTo();
        }
        else
        {
            if (dialogText2 != null)
            {
                DialogueManager.instance.EnterDialogMode(dialogText2);
            }
            else
            {
                Debug.Log("Post-Mission talk");
            }
        }
        return false;
    }
}
