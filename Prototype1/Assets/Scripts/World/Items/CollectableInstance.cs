using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableInstance : InteractableBehaviorTemplate, ISaveable
{
    private bool collected = false;
    [SerializeField] string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
    public override bool Interact()
    {
        collected = true;
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
