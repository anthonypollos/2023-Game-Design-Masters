using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectMissionBehavior : MissionBehavior
{
    [SerializeField] List<GameObject> items;
    List<MissionCollectable> missionCollectables;
    int collected;

    new private void Start()
    {
        collected = 0;
        missionCollectables = new List<MissionCollectable>();
        foreach(GameObject item in items)
        {
            missionCollectables.Add(item.GetComponent<MissionCollectable>());
        }
        foreach(MissionCollectable missionCollectable in missionCollectables)
        {
            missionCollectable.SetMission(this);
        }
        base.Start();
    }

    public void Collected()
    {
        collected++;
        if(collected >= missionCollectables.Count)
        {
            OnComplete();
        }
    }

    public override (string, bool) GetMissionText()
    {
        string editedMissionText = missionText;
        if(!triggered)
        {
            editedMissionText += " (" + collected + "/" + missionCollectables.Count + ")";
        }
        return (editedMissionText, base.GetMissionText().Item2);
    }

    public override void OnComplete()
    {
        foreach(GameObject item in items)
        {
            item.SetActive(false);
        }
        base.OnComplete();
    }
}
