using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubCollectibleInstance : MonoBehaviour, ISaveable
{
    [SerializeField] string id;
    private bool collected = false;

    private void Awake()
    {
    }

    public void LoadData(SavedValues savedValues)
    {
        savedValues.collectables.TryGetValue(id, out collected);
        if (!collected)
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
