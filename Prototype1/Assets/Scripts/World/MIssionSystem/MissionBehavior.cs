using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionBehavior : MonoBehaviour
{
    [SerializeField] List<GameObject> toggleOnFinish;
    private List<IToggleable> toggles;
    [SerializeField] string missionText;
    public Vector3 checkPointLocation;
    protected MissionFolder folder;
    protected bool triggered;

    protected void Start()
    {
        if (toggles == null)
        {
            toggles = new List<IToggleable>();
            foreach (GameObject temp in toggleOnFinish)
            {
                IToggleable toggle = temp.GetComponentInChildren<IToggleable>();
                if (toggle != null) { toggles.Add(toggle); }
            }
        }
    }
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

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            OnTriggered();
            triggered = true;
        }
    }

    public void QuickSetToggles()
    {
        if (toggles == null)
        {
            toggles = new List<IToggleable>();
            foreach (GameObject temp in toggleOnFinish)
            {
                IToggleable toggle = temp.GetComponentInChildren<IToggleable>();
                if (toggle != null) { toggles.Add(toggle); }
            }
        }
    }

    public virtual void OnComplete()
    {
        triggered = true;
        if(toggles.Count> 0) 
        foreach(IToggleable toggle in toggles)
        {
            toggle.Toggle();
        }
        folder.MissionComplete(this);
    }

}
