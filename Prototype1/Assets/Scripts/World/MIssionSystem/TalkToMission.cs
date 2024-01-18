using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkToMission : MissionBehavior
{

    [SerializeField] List<GameObject> npcsToTalkTo;
    List<NPCMission> npcs;
    int talkedTo;

    new private void Start()
    {

        if (npcs == null)
        {
            QuickSetNPCs();
        }
        base.Start();
    }

    private void QuickSetNPCs()
    {
        talkedTo = 0;
        npcs = new List<NPCMission>();
        foreach (GameObject item in npcsToTalkTo)
        {
            npcs.Add(item.GetComponent<NPCMission>());
        }
        foreach (NPCMission npc in npcs)
        {
            npc.SetMission(this);
        }
    }

    public void TalkedTo()
    {
        talkedTo++;
        if (talkedTo >= npcs.Count)
        {
            OnComplete();
        }
    }

    public override (string, bool) GetMissionText()
    {
        if (npcs == null)
        {
            QuickSetNPCs();
        }
        string editedMissionText = missionText;
        if (!triggered)
        {
            editedMissionText += " (" + talkedTo + "/" + npcs.Count + ")";
        }
        return (editedMissionText, base.GetMissionText().Item2);
    }

    public override void OnComplete()
    {
        QuickSetNPCs();
        foreach (NPCMission npc in npcs)
        {
            npc.TalkedTo();
        }
        base.OnComplete();
    }
}

