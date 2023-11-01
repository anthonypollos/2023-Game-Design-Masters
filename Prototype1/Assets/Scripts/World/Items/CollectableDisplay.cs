using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableDisplay : InteractableBehaviorTemplate, ISaveable
{ 
    private bool collected = false;
    [SerializeField] [Tooltip("Copy from corresponding CollectableInstance script")]string id;
    //[SerializeField] GameObject displayUI;

    public override bool Interact()
    {
        //displayUI.SetActive(true);
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
