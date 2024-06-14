using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager instance { get; private set; }

    [SerializeField] private TextMeshProUGUI collectName, collectDesc;
    [SerializeField] private TextMeshProUGUI noteName;

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
    public void DisplayCollectible(int index, string name, string desc)
    {
        tempIndex = index;
        tempName = name;
        tempDesc = desc;

        Invoke("DisplayCollectible", 0.15f);
    }

    private void DisplayCollectible()
    {
        pickupUI.SetActive(true);

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
    public void DisplayNote(int index, string name, string[] desc)
    {
        tempIndex = index;
        tempName = name;
        tempDescList = desc;

        Invoke("DisplayNote", 0.15f);
    }

    private void DisplayNote()
    {
        pickupUI.SetActive(true);

        noteName.text = name;

        if (multiPage != null)
            multiPage.SetPage(0, tempDescList);

        anim.SetBool("Collectible", false);
        anim.SetInteger("CollectIndex", tempIndex);

        anim.SetTrigger("StartAnim");
    }

    public void CloseMenu()
    {
        Invoke("DisableMenu", 1.0f);
    }

    private void DisableMenu()
    {
        pickupUI.SetActive(false);
    }
}
