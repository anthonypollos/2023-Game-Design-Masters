using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableDisplay : InteractableBehaviorTemplate, ISaveable
{ 
    private bool collected = false;
    [SerializeField] [Tooltip("Copy from corresponding CollectableInstance script")]string id;
    //[SerializeField] GameObject displayUI;

    [SerializeField] TextAsset dialogText;

    public override bool Interact()
    {
        DialogueManager.instance.EnterDialogMode(dialogText);
        return false;
    }

    public void LoadData(SavedValues savedValues)
    {
        savedValues.collectables.TryGetValue(id, out collected);
        if (!collected)
        {
            gameObject.SetActive(false);
        }
    }

    public void SaveData(ref SavedValues savedValues)
    {
        return;
    }

}
