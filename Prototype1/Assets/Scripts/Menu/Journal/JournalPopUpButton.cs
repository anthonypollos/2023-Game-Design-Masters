using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JournalPopUpButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptText;
    public string journalPageToLoad;

    private UINavManager journalNavManager;

    // Start is called before the first frame update
    void Awake()
    {
        journalNavManager = GameObject.Find("Journal Manager").GetComponent<UINavManager>();
    }

    public void SetButton(string buttonText, string pageToLoad)
    {
        promptText.text = buttonText;
        journalPageToLoad = pageToLoad;
    }

    public void OpenToPage()
    {
        if(journalNavManager != null)
        {
            //journalNavManager.OpenMainMenu();
            FindObjectOfType<GameController>().ToggleJournal();
            journalNavManager.OpenSubMenu(journalPageToLoad);
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}

