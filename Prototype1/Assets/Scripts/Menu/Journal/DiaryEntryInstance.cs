using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiaryEntryInstance : MonoBehaviour, ISaveable
{
    public string id;

    public bool isHub = false;

    public enum PrevLevel { Tutorial, Town, Railyard };
    public PrevLevel prevLevel;

    [HideInInspector] public bool collected;

    public void CollectDiaryEntry()
    {
        collected = true;

        SaveLoadManager.instance.SaveGame();
    }

    public void LoadData(SavedValues savedValues)
    {
        savedValues.collectables.TryGetValue(id, out collected);
        if (collected)
        {
            gameObject.SetActive(false);
        }
    }

    public void SaveData(ref SavedValues savedValues)
    {
        if (savedValues.collectables.ContainsKey(id))
        {
            savedValues.collectables.Remove(id);
        }
        savedValues.collectables.Add(id, collected);
    }

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
}
