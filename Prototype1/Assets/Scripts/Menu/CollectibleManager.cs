using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager instance { get; private set; }

    [SerializeField] private TextMeshProUGUI collectName, collectDesc;
    [SerializeField] private TextMeshProUGUI noteName;

    [SerializeField] private UINavManager navManager;

    [SerializeField] private GameObject pickupUI;

    [SerializeField] private Animator anim;
    [SerializeField] private MultiPageText multiPage;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one collectible manager");
            Destroy(this);
        }
        instance = this;
    }

    int tempIndex;
    string tempName;
    string tempDesc;
    string[] tempDescList;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="desc"></param>
    public void DisplayCollectible(int index, string itemName, string desc)
    {
        tempIndex = index;
        tempName = itemName;
        tempDesc = desc;

        Invoke("DisplayCollectible", 0.15f);
    }

    private void DisplayCollectible()
    {
        //pickupUI.SetActive(true);
        navManager.CloseMainMenu();
        navManager.OpenMainMenu();

        collectName.text = tempName;
        collectDesc.text = tempDesc;

        anim.SetBool("Collectible", true);
        anim.SetInteger("CollectIndex", tempIndex);


        anim.SetTrigger("StartAnim");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="desc"></param>
    public void DisplayNote(int index, string itemName, string[] desc)
    {
        tempIndex = index;
        tempName = itemName;
        tempDescList = desc;

        Invoke("DisplayNote", 0.15f);
    }

    private void DisplayNote()
    {
        //pickupUI.SetActive(true);

        navManager.CloseMainMenu();
        navManager.OpenMainMenu();

        noteName.text = tempName;

        if (multiPage != null)
            multiPage.SetPage(0, tempDescList);

        anim.SetBool("Collectible", false);
        anim.SetInteger("CollectIndex", tempIndex);

        anim.SetTrigger("StartAnim");
    }
}
