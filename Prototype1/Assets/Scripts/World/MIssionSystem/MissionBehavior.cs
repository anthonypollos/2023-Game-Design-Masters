using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

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

    [Header("Dialogue on completion (LEAVE EMPTY IF NO DIALOGUE)")]
    [Header("Default Texts (no levels beaten)")]
    [SerializeField] TextAsset dialogueText1;
    [Header("Text based on level(s) completed")]
    [Tooltip("Prioritized from top down")]
    [SerializeField] List<TextAssets2> dialogueTexts;
    TextAsset initialDialogue;

    //[SerializeField] private JukeBox jukebox;
    [SerializeField] private EventReference objectiveSound;

    private void Awake()
    {
        //jukebox.SetTransform(transform);
    }

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
        SavedValues savedValues = SaveLoadManager.instance.GetCopy();
        foreach (TextAssets2 asset in dialogueTexts)
        {
            bool levelFinished;
            if (savedValues.levels.TryGetValue(asset.levelName, out levelFinished))
            {
                if (levelFinished)
                {
                    initialDialogue = asset.initialDialogue;
                    break;
                }

            }
        }
        if (initialDialogue == null)
        {
            initialDialogue = dialogueText1;
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
        AudioManager.instance.PlayOneShot(objectiveSound, this.transform.position);
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
            Debug.Log(gameObject.name + ": completed");
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
            //AudioManager.instance.PlayOneShot(objectiveSound, this.transform.position);
            folder.MissionComplete(this);
            //jukebox.PlaySound(0);
            //AudioManager.instance.PlayOneShot(objectiveSound, this.transform.position);
            if (initialDialogue != null)
                DialogueManager.instance.EnterDialogMode(initialDialogue);
        }
    }



}
