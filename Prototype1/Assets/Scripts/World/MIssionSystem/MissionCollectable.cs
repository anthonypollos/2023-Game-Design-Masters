using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCollectable : InteractableBehaviorTemplate, ISaveable
{
    bool collected = false;
    [SerializeField] string id;
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
        collected = true;
        SaveLoadManager.instance.SaveGame();
        return true;
    }

    public void SaveData(ref SavedValues savedValues)
    {
        if (id != null || id != string.Empty)
        {
            if (savedValues.collectables.ContainsKey(id))
            {
                savedValues.collectables.Remove(id);
            }
            savedValues.collectables.Add(id, collected);
        }
    }

    public void LoadData(SavedValues savedValues)
    {
        if (id != null || id != string.Empty)
        {
            savedValues.collectables.TryGetValue(id, out collected);
        }
    }
}
