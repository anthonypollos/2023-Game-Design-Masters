using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCollectable : InteractableBehaviorTemplate
{
    CollectMissionBehavior mission;

    public void SetMission(CollectMissionBehavior mission)
    {
        this.mission = mission;
    }

    public override bool Interact()
    {
        mission.Collected();
        return true;
    }

}
