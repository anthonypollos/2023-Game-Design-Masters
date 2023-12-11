using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionBehavior : MonoBehaviour
{
    [Tooltip("Objects with a toggleable to be toggled when finished on it or its children")]
    [SerializeField] List<GameObject> toggleOnFinish;
    private List<IToggleable> toggles;
    [Tooltip("Game Objects you want to be toggled on or off completely when finished")]
    [SerializeField] List<GameObject> toggleIfActiveOnFinish;
    [SerializeField] protected string missionText;
    public Vector3 checkPointLocation;
    protected IMissionContainer folder;
    protected bool triggered;
    protected bool completed;

    protected void Start()
    {
        completed = false;
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
        if (!completed)
        {
            QuickSetToggles();
            triggered = true;
            completed = true;
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
            if (toggleIfActiveOnFinish != null)
            {
                if (toggleIfActiveOnFinish.Count > 0)
                {
                    foreach (GameObject go in toggleIfActiveOnFinish)
                    {
                        go.SetActive(!go.activeInHierarchy);
                    }
                }
            }
            folder.MissionComplete(this);
        }
    }



}
