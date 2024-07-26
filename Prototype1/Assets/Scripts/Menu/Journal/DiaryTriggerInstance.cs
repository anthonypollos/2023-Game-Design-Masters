using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class DiaryTriggerInstance : MonoBehaviour, ISaveable
{
    private bool collected = false;

    [SerializeField] string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            CollectTrigger();
        }
    }

    public void CollectTrigger()
    {
        collected = true;

        DiaryManager.instance.CollectDiaryUI();

        gameObject.SetActive(false);

        Invoke("SaveDelay", 0.3f);
    }

    private void SaveDelay()
    {
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
}
