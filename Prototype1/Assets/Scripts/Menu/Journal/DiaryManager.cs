using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class DiaryManager : MonoBehaviour
{
    public static DiaryManager instance;

    [SerializeField] private EventReference collectsound;

    [SerializeField] private DiaryEntryInstance[] entries;
    private DiaryTriggerInstance[] triggers;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        entries = FindObjectsOfType<DiaryEntryInstance>();
        triggers = FindObjectsOfType<DiaryTriggerInstance>();
    }

    public void CollectDiaryEntry(string entryID)
    {
        foreach (DiaryEntryInstance entry in entries)
        {
            if (entry.id.Equals(entryID) && !entry.collected)
            {
                entry.CollectDiaryEntry();
                CollectDiaryUI();
            }
        }
        //SaveLoadManager.instance.SaveGame();
    }

    public void CollectAllEntries()
    {
        foreach (DiaryEntryInstance entry in entries)
        {
            if (!entry.collected)
                entry.CollectDiaryEntry();
        }

        if(triggers != null)
        {
            foreach (DiaryTriggerInstance trigger in triggers)
            {
                if (trigger.gameObject.activeInHierarchy)
                    trigger.CollectTrigger();
            }

        }
    }

    public void CollectDiaryEntryHub()
    {
        print("Hub entry");

        SavedValues temp = SaveLoadManager.instance.GetCopy();

        bool beatRail = temp.levels.TryGetValue("C_ArtPass_railyard_v2", out beatRail);


        bool beatTown = temp.levels.TryGetValue("Town_Connor_Art_Pass", out beatTown);

        bool beatTut = temp.levels.TryGetValue("Tutorial_new", out beatTut);

        if (beatRail)
            SearchEntries("Railyard");
        if (beatTown)
            SearchEntries("Town");
        if (beatTut)
            SearchEntries("Tutorial");

        print(" hub Beat Rail: " + beatRail);
        print("hub BeatTown: " + beatTown);
        print("hub BeatTut: " + beatTut);
    }

    private void SearchEntries(string key)
    {
        foreach (DiaryEntryInstance entry in entries)
        {
            if (entry.prevLevel.ToString().Equals(key) && !entry.collected)
            {
                entry.CollectDiaryEntry();
                CollectDiaryUI();
            }
        }
    }

    public void CollectDiaryUI()
    {
        if(JournalPopUpSpawner.instance != null)
            JournalPopUpSpawner.instance.SpawnDiaryPopUp();
        AudioManager.instance.PlayOneShot(collectsound, this.transform.position);
    }
}

/*
 *     [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
*/