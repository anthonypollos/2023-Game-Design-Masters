/*
 * Avery
 */
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JournalItem : MonoBehaviour
{
    // THIS IS TEMP!!! UNTIL TIED IN W SAVE SYSTEM!!!
    public bool isFound = false;

    [Header("General Variables")]

    [Tooltip("Item's save data ID")] public string itemID;

    public string itemName;

    /// <summary>
    /// Name to display if player has not found item yet
    /// </summary>
    private string nameNotFound = "?????";

    [TextArea(5, 10)]
    public string[] itemDescription;

    /// <summary>
    /// Script for descriptions that require multiple "pages" of text, should only be notes
    /// </summary>
    private MultiPageText multiPage;

    /// <summary>
    /// Used to create descriptionNotFound
    /// </summary>
    public enum ItemType { Note, Collectible, Character, Enemy };
    public ItemType itemType;

    public enum ItemLocation { Tutorial, Hub, Town, Railyard }
    public ItemLocation itemLocation;

    /// <summary>
    /// Description to display if player has not found item yet
    /// </summary>
    private string descriptionNotFound;

    [Header("Animator Variables")]

    /// <summary>
    /// Journal Menu animator controller
    /// </summary>
    private Animator anim;

    [Tooltip("Name of matching Index variable in Journal Controller BlendTree")]
    public string animVariableName;

    [Tooltip("Index of matching item in Journal Controller BlendTree")]
    public float animIndex;

    private Color colorFound = new Color(0.95f, 0.9f, 0.84f, 1);
    private Color colorNotFound = new Color(0.25f, 0.2f, 0.15f, 0.75f);

    [Header("UI Display Variables")]
    public Image itemImage;
    public TextMeshProUGUI headerText, descriptionText;

    /// <summary>
    /// Dispaly text of item button
    /// </summary>
    private TextMeshProUGUI buttonText;

    void Start()
    {
        SetDescriptionNotFoundText();

        multiPage = FindObjectOfType<MultiPageText>();

        anim = GameObject.Find("Journal Menu").GetComponent<Animator>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        /*
         * IF ITEM NOT FOUND! Need to set button text to nameNotFound
         */
    }

    public void SelectItem()
    {
        if(isFound) //TEMP; if found
        {
            headerText.text = itemName;
            descriptionText.text = itemDescription[0];

            if(itemImage != null)
                itemImage.color = colorFound;
        }
        else //TEMP, if not found
        {
            headerText.text = nameNotFound;
            descriptionText.text = descriptionNotFound;

            if (itemImage != null)
                itemImage.color = colorNotFound;
        }

        // temp fix
        if(anim == null)
            anim = GameObject.Find("Journal Menu").GetComponent<Animator>();

        if (animVariableName != null)
            anim.SetFloat(animVariableName, animIndex);
    }


    /// <summary>
    /// For descriptions that require multiple pages of text; display a certain "page" of text
    /// </summary>
    /// <param name="value"></param>
    public void SetPage(int value)
    {
        // temp fix
        if(multiPage == null)
            multiPage = FindObjectOfType<MultiPageText>(true);

        multiPage.SetPage(value, itemDescription);
    }

    private void SetDescriptionNotFoundText()
    {
        string text = "";

        switch (itemType)
        {
            case ItemType.Enemy:
                text += "Meet this foe ";
                break;
            case ItemType.Character:
                text += "Meet this ally ";
                break;
            case ItemType.Note:
                text += "Find this document ";
                break;
            case ItemType.Collectible:
                text += "Find this item ";
                break;
        }

        switch (itemLocation)
        {
            case ItemLocation.Tutorial:
                text += "in the [Tutorial Name Here].";
                break;
            case ItemLocation.Hub:
                text += "at The Last Spike.";
                break;
            case ItemLocation.Town:
                text += "in the Town.";
                break;
            case ItemLocation.Railyard:
                text += "in the Railyard.";
                break;
        }

        descriptionNotFound = text;
    }
}