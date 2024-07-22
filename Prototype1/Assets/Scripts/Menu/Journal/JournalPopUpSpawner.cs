using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalPopUpSpawner : MonoBehaviour
{
    public static JournalPopUpSpawner instance;

    [SerializeField] private GameObject popUp;
    [SerializeField] private GameObject spawnParent;

    private void Start()
    {
        if (instance == null)
            instance = this;
    }

    public void SpawnJournalPopUp(string popUpText, string pageToLoad)
    {
        GameObject toSpawn = popUp;

        toSpawn.GetComponent<JournalPopUpButton>().SetButton(popUpText, pageToLoad);

        Instantiate(toSpawn, spawnParent.transform);
    }

    public void SpawnCollectPopUp()
    {
        SpawnJournalPopUp("View Souvenir in Journal", "Collectibles");
    }

    public void SpawnNotePopUp()
    {
        SpawnJournalPopUp("View Note in Journal", "Notes");
    }

    public void SpawnDiaryPopUp()
    {
        SpawnJournalPopUp("View New Entry in Journal", "Diary");
    }
}
