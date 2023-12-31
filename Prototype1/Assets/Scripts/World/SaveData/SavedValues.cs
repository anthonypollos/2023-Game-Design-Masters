using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedValues
{
    public SerializeableDictionary<string, bool> collectables;
    public SerializeableDictionary<string, bool> levels;
    public string currentLevel;
    public List<bool> currentLevelMissionStatuses;
    //public Vector3 checkPointLocation;

    public SavedValues()
    {
        collectables = new SerializeableDictionary<string, bool>();
        levels = new SerializeableDictionary<string, bool>();
        //currentLevel = "";
        currentLevelMissionStatuses = new List<bool>();
        //checkPointLocation = Vector3.zero;
    }
}
