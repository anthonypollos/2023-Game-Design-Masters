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
        
        if (missionCollectables == null)
        {
            QuickSetCollectables();
        }
        base.Start();
    }

    private void QuickSetCollectables()
    {
        collected = 0;
        missionCollectables = new List<MissionCollectable>();
        foreach (GameObject item in items)
        {
            missionCollectables.Add(item.GetComponent<MissionCollectable>());
        }
        foreach (MissionCollectable missionCollectable in missionCollectables)
        {
            missionCollectable.SetMission(this);
        }
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
        if(missionCollectables == null)
        {
            QuickSetCollectables();
        }
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
