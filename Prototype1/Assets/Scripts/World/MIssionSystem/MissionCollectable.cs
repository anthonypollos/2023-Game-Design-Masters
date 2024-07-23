using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class MissionCollectable : InteractableBehaviorTemplate, ISaveable
{
    bool collected = false;
    [SerializeField] string id;
    CollectMissionBehavior mission;
    [SerializeField] TextAsset textToDisplay;
    [SerializeField] bool isNote;

    [SerializeField] int collectAnimIndex;
    [SerializeField] string itemName;
    [TextArea(5, 20)] [SerializeField] string[] description;

    [SerializeField] private EventReference collectsound;
    [SerializeField] private EventReference barksound;

    public void SetMission(CollectMissionBehavior mission)
    {
        this.mission = mission;
    }

    public override bool Interact()
    {
        //if (textToDisplay != null)
        //{
        //    DialogueManager.instance.EnterDialogMode(textToDisplay);
        //}

        if (/*textToDisplay != null &&*/ !isNote && id != "")
        {
            //DialogueManager.instance.EnterDialogMode(textToDisplay);
            CollectibleManager.instance.DisplayCollectible(collectAnimIndex, itemName, description[0]);
        }
        else if (/*textToDisplay != null &&*/ isNote && id != "")
        {
            //NoteManager.instance.EnterDialogMode(textToDisplay);
            CollectibleManager.instance.DisplayNote(16, itemName, description);
        }

        else
        {
            Debug.Log("Play text here");
        }
        mission.Collected();
        collected = true;
        AudioManager.instance.PlayOneShot(collectsound, this.transform.position);
        AudioManager.instance.PlayOneShot(barksound, this.transform.position);
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
