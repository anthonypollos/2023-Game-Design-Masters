using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableInstance : InteractableBehaviorTemplate, ISaveable
{
    private bool collected = false;
    [SerializeField] string id;
    [SerializeField] TextAsset textToDisplay;
    [SerializeField] bool isNote;

    [SerializeField] private JukeBox jukebox;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void Awake()
    {
        jukebox.SetTransform(transform);
    }
    public override bool Interact()
    {
        if (textToDisplay != null && !isNote)
        {
            DialogueManager.instance.EnterDialogMode(textToDisplay);
        }
        else if (textToDisplay != null && isNote)
        {
            NoteManager.instance.EnterDialogMode(textToDisplay);
        }
        else
        {
            Debug.Log("Play text here");
        }
        collected = true;
        jukebox.PlaySound(0);
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
