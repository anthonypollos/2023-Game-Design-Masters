using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelProgressionInstance : MonoBehaviour
{
    [SerializeField]
    List<string> priorLevelsRequired;
    [SerializeField]
    [Tooltip("Copy from corresponding CollectableInstance script")]
    List<string> collectableIDs;
    [SerializeField]
    TextMeshProUGUI collectableText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        SavedValues values = SaveLoadManager.instance.GetCopy();
        if(!CheckLevels(values))
        {
            gameObject.SetActive(false);
        }
        else
        {
            CheckCollectables(values);
        }
    }

    bool CheckLevels(SavedValues values)
    {
        if(priorLevelsRequired == null)
        {
            return true;
        }
        if(priorLevelsRequired.Count == 0)
        {
            return true;
        }
        foreach (var level in priorLevelsRequired)
        {
            values.levels.TryGetValue(level, out bool temp);
            if (!temp) 
            {
                return false;
            }
        }
        return true;
    }

    void CheckCollectables(SavedValues values)
    {
        int count = 0;
        foreach (var id in collectableIDs)
        {
            values.collectables.TryGetValue(id, out bool temp);
            if (temp)
            {
                count++;
            }
        }
        collectableText.text = count + "/" + collectableIDs.Count + " collectibles";
    }
}
