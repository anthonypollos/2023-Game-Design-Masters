using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiaryPageInstance : MonoBehaviour 
{
    private bool isFound = false;
    [SerializeField] Entry[] entries;

    [SerializeField] MultiPageDiary multiPage;

    [SerializeField] private GameObject pageButton;

    private void OnEnable()
    {
        foreach (Entry entry in entries)
        {
            SavedValues temp = GameController.instance.savedValuesInstance;
            bool exists;

            exists = temp.collectables.TryGetValue(entry.entryID, out isFound);
            if (!exists)
                isFound = false;

            if (!isFound)// || // multiPage.currentPage != entry.displayOnPage)
                entry.entry.SetActive(false);
            else
            {
                pageButton.SetActive(true);

                entry.entry.SetActive(true);

                if (entry.displayOnPage > multiPage.maxPages)
                    multiPage.maxPages++;

                multiPage.CheckMultiPage();
            }
        }
    }

    public void CheckPage()
    {
        foreach (Entry entry in entries)
        {
            SavedValues temp = GameController.instance.savedValuesInstance;
            bool exists;

            exists = temp.collectables.TryGetValue(entry.entryID, out isFound);
            if (!exists)
                isFound = false;

            if(isFound)
            {
                if (entry.displayOnPage == multiPage.currentPage)
                    entry.entry.SetActive(true);
                else if (entry.displayOnPage != multiPage.currentPage)
                    entry.entry.SetActive(false);
            }
        }
    }

    public void ResetPage()
    {
        foreach (Entry entry in entries)
        {
            SavedValues temp = GameController.instance.savedValuesInstance;
            bool exists;

            exists = temp.collectables.TryGetValue(entry.entryID, out isFound);
            if (!exists)
                isFound = false;

            if (isFound)
            {
                if (entry.displayOnPage == 1)
                    entry.entry.SetActive(true);
                else
                    entry.entry.SetActive(false);
            }

            multiPage.SetPage(0);
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
                pageButton.SetActive(true);

                entry.entry.SetActive(true);

                if (entry.displayOnPage >= multiPage.maxPages)
                    multiPage.maxPages++;

                multiPage.CheckMultiPage();
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


