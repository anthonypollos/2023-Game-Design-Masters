using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class DiaryInstance : MonoBehaviour, ISaveable
{
    private bool collected = false;
    [SerializeField] string id;
    [TextArea(5, 20)] [SerializeField] string[] description;
    [SerializeField] private EventReference collectsound;


    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            collected = true;

            AudioManager.instance.PlayOneShot(collectsound, this.transform.position);
            SaveLoadManager.instance.SaveGame();
        }
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
