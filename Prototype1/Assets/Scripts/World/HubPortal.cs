using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class HubPortal : InteractableBehaviorTemplate, ISaveable
{
    [SerializeField] string worldName;
    public override bool Interact()
    {
        SceneManager.LoadScene(worldName);
        return false;
    }

    public void LoadData(SavedValues savedValues)
    {
        bool temp = false;
        savedValues.levelsCompleted.TryGetValue(worldName, out temp);
        if(temp)
        {
            GetComponent<MeshRenderer>().material.color = Color.green;
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    public void SaveData(ref SavedValues savedValues)
    {
        bool temp = false;
        if(savedValues.levelsCompleted.ContainsKey(worldName))
        {
            temp = savedValues.levelsCompleted[worldName];
            savedValues.levelsCompleted.Remove(worldName);
        }
        savedValues.levelsCompleted.Add(worldName, temp);

    }
}
