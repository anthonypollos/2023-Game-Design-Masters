using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiaryInstance : MonoBehaviour 
{
    private bool isFound = false;
    [SerializeField] Entry[] entries;

    [SerializeField] MultiPageDiary multiPage;

    private void OnEnable()
    {
        foreach (Entry entry in entries)
        {
            SavedValues temp = GameController.instance.savedValuesInstance;
            bool exists;

            exists = temp.collectables.TryGetValue(entry.entryID, out isFound);
            if (!exists)
                isFound = false;

            if (!isFound || multiPage.currentPage != entry.displayOnPage)
                entry.entry.SetActive(false);
            else
            {
                entry.entry.SetActive(true);

                if (entry.displayOnPage >= multiPage.maxPages)
                    multiPage.maxPages = entry.displayOnPage + 1;
            }
        }
    }

    public void CheckEnable()
    {
        foreach (Entry entry in entries)
        {
            if (!isFound || multiPage.currentPage != entry.displayOnPage)
                entry.entry.SetActive(false);
            else
            {
                entry.entry.SetActive(true);
                if (entry.displayOnPage >= multiPage.maxPages)
                    multiPage.maxPages = entry.displayOnPage + 1;
            }
        }
    }

    [System.Serializable]
    public class Entry
    {
        public string entryID;
        public int displayOnPage;
        public GameObject entry;
    }
}


