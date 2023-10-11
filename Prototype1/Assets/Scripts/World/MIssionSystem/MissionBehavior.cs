using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionBehavior : MonoBehaviour
{
    [SerializeField] IToggleable[] toggleOnFinish;
    [SerializeField] string missionText;
    protected MissionFolder folder;
    protected bool completed;

    public void SetFolder(MissionFolder folder)
    {
        this.folder = folder;
    }

    public string GetMissionText()
    {
        return missionText;
    }
    
    protected virtual void OnTriggered()
    {
        OnComplete();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnTriggered();
        }
    }

    protected virtual void OnComplete()
    {
        foreach(IToggleable toggle in toggleOnFinish)
        {
            toggle.Toggle();
        }

    }
}
