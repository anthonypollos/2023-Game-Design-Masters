using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class CollectableInstance : InteractableBehaviorTemplate, ISaveable
{
    private bool collected = false;
    [SerializeField] string id;
    [SerializeField] TextAsset textToDisplay;
    [SerializeField] bool isNote;

    [SerializeField] int collectAnimIndex;
    [SerializeField] string itemName;
    [TextArea(5, 20)] [SerializeField] string[] description;
    [SerializeField] private EventReference collectsound;
    [SerializeField] private EventReference collectBark;
    [SerializeField] private VoiceClip collectBarkVoiceClip;

    //[SerializeField] private JukeBox jukebox;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void Awake()
    {
        //jukebox.SetTransform(transform);
    }
    public override bool Interact()
    {
        if (/*textToDisplay != null &&*/ !isNote)
        {
            //DialogueManager.instance.EnterDialogMode(textToDisplay);
            CollectibleManager.instance.DisplayCollectible(collectAnimIndex, itemName, description[0]);
        }
        else if (/*textToDisplay != null &&*/ isNote)
        {
            //NoteManager.instance.EnterDialogMode(textToDisplay);
            CollectibleManager.instance.DisplayNote(16, itemName, description);
        }
        else
        {
            Debug.Log("Play text here");
        }
        collected = true;

        //jukebox.PlaySound(0);
        AudioManager.instance.PlayOneShot(collectsound, this.transform.position);
        if (collectBarkVoiceClip.subtitle == null || collectBarkVoiceClip.subtitle == string.Empty)
        {
            AudioManager.instance.PlayOneShot(collectBark, this.transform.position);
        }
        else
        {
            SubtitleManager.instance.StartDialog(collectBarkVoiceClip, true);
        }
        SaveLoadManager.instance.SaveGame();
        return true;
    }


    public void LoadData(SavedValues savedValues)
    {
        savedValues.collectables.TryGetValue(id, out collected);
        if(collected)
        {
            gameObject.SetActive(false);
        }
    }

    public void SaveData(ref SavedValues savedValues)
    {
        if(savedValues.collectables.ContainsKey(id))
        {
            savedValues.collectables.Remove(id);
        }
        savedValues.collectables.Add(id, collected);
    }


}
