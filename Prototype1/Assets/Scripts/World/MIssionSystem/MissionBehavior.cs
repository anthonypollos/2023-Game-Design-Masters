using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionBehavior : MonoBehaviour
{
    [SerializeField] List<GameObject> toggleOnFinish;
    private List<IToggleable> toggles;
    [SerializeField] protected string missionText;
    public Vector3 checkPointLocation;
    protected IMissionContainer folder;
    protected bool triggered;

    protected void Start()
    {
        toggles = new List<IToggleable>();
        if (toggles == null && toggleOnFinish.Count>0)
        {
            //toggles = new List<IToggleable>();
            foreach (GameObject temp in toggleOnFinish)
            {
                IToggleable toggle = temp.GetComponentInChildren<IToggleable>();
                if (toggle != null) { toggles.Add(toggle); }
            }
        }
    }
    public void SetFolder(IMissionContainer folder)
    {
        this.folder = folder;
    }

    public virtual (string, bool) GetMissionText()
    {
        return (missionText, false);
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
        if (toggles == null && toggleOnFinish.Count>0)
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
        if (toggles != null)
        {
            if (toggles.Count > 0)
            {
                foreach (IToggleable toggle in toggles)
                {
                    toggle.Toggle();
                }
            }
        }
        folder.MissionComplete(this);
    }



}
