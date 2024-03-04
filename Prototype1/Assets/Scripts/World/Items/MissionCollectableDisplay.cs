using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCollectableDisplay : InteractableBehaviorTemplate
{
    [SerializeField] string requiredLevel;
    [SerializeField] TextAsset dialogText;

    private void Start()
    {
        SavedValues savedValues = SaveLoadManager.instance.GetCopy();
        bool temp;
        if(savedValues.levels.TryGetValue(requiredLevel, out temp))
        {
            if(temp)
            {
                return;
            }
        }
        gameObject.SetActive(false);
    }
    public override bool Interact()
    {
        DialogueManager.instance.EnterDialogMode(dialogText);
        return false;
    }
}
