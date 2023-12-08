using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCollectable : InteractableBehaviorTemplate
{
    CollectMissionBehavior mission;
    [SerializeField] TextAsset textToDisplay;

    public void SetMission(CollectMissionBehavior mission)
    {
        this.mission = mission;
    }

    public override bool Interact()
    {
        if (textToDisplay != null)
        {
            DialogueManager.instance.EnterDialogMode(textToDisplay);
        }
        else
        {
            Debug.Log("Play text here");
        }
        mission.Collected();
        return true;
    }

}
