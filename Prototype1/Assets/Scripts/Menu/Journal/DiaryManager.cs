using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class DiaryManager : MonoBehaviour
{
    public static DiaryManager instance;

    [SerializeField] private EventReference collectsound;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void CollectDiaryEntry(string id)
    {
        SavedValues temp = GameController.instance.savedValuesInstance;

        if (temp.collectables.ContainsKey(id))
            temp.collectables.Remove(id);
        temp.collectables.Add(id, true);

        AudioManager.instance.PlayOneShot(collectsound, this.transform.position);


        // trigger animation, sound effect
    }
}

/*
 *     [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
*/