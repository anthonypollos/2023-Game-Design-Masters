using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class HubPortal : InteractableBehaviorTemplate, ISaveable
{
    [SerializeField] string worldName;
    FMOD.Studio.Bus MasterBus;
    public override bool Interact()
    {
        MasterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
        //MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        SceneManager.LoadScene(worldName);
        MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        return false;
    }

    public void LoadData(SavedValues savedValues)
    {
        bool temp = false;
        savedValues.levels.TryGetValue(worldName, out temp);
        if (savedValues.currentLevel == "HubScene")
        {
            if (temp)
            {
                GetComponent<MeshRenderer>().material.color = Color.green;
            }
            else
            {
                GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = Color.cyan;
        }
    }

    public void SaveData(ref SavedValues savedValues)
    {
        bool temp = false;
        if(savedValues.levels.ContainsKey(worldName))
        {
            temp = savedValues.levels[worldName];
            savedValues.levels.Remove(worldName);
        }
        savedValues.levels.Add(worldName, temp);

    }
}
