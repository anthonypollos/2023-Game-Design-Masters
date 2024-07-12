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
    [Tooltip("Nothing = always available, itemID = requires ID to be collected, levelCheck requires level (ItemLocation variable) to be completed")]
    enum CollectedCheck {nothing, itemID, levelCheck, finalCutscene}
    [SerializeField] CollectedCheck collectedCheck;

    [Header("General Variables")]

    [Tooltip("Item's save data ID")] public string itemID;

    public string itemName;

    /// <summary>
    /// Name to display if player has not found item yet
    /// </summary>
    private string nameNotFound = "?????";

    [TextArea(5, 20)]
    public string[] itemDescription;

    /// <summary>
    /// Script for descriptions that require multiple "pages" of text, should only be notes
    /// </summary>
    private MultiPageText multiPage;

    /// <summary>
    /// Used to create descriptionNotFound
    /// </summary>
    public enum ItemType { Note, Collectible, Character, Enemy, Cutscene };
    public ItemType itemType;

    public enum ItemLocation { Tutorial, Hub, Town, Railyard, Final }
    public ItemLocation itemLocation;


    [Tooltip("Should this item be the default selected entry in its sub-menu?")]
    public bool selectOnStart = false;

    //Change this value in relation to the above locations to the scene name
    private string[] levels = new string[4]{"Tutorial_new", "HubScene", "Town_Connor_Art_Pass", "C_ArtPass_railyard_v2"};

    /// <summary>
    /// Description to display if player has not found item yet
    /// </summary>
    private string[] descriptionNotFound = new string[1];

    [Header("Animator Variables")]


    [Tooltip("Name of matching Index variable in Journal Controller BlendTree")]
    public string animVariableName;

    [Tooltip("Index of matching item in Journal Controller BlendTree")]
    public float animIndex;

    /// <summary>
    /// Journal Menu animator controller
    /// </summary>
    private Animator anim;

    private Color colorFound = new Color(0.95f, 0.9f, 0.84f, 1);
    private Color colorNotFound = new Color(0.25f, 0.2f, 0.15f, 0.75f);

    [Header("UI Display Variables")]
    public Image itemImage;
    public TextMeshProUGUI headerText, descriptionText;

    private RectTransform underline;
    [SerializeField] private float underlineLengthFound;
    private float underlineLengthNotFound = 120;

    [SerializeField] private GameObject cutsceneButton;

    /// <summary>
    /// Dispaly text of item button
    /// </summary>
    private TextMeshProUGUI buttonText;

    void Start()
    {
        //SetDescriptionNotFoundText();

        anim = GameObject.Find("Journal Menu").GetComponent<Animator>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        //descriptionNotFoundArray = new string[itemDescription.Length];
        //for (int i = 0; i < itemDescription.Length; i++)
        //{
        //    descriptionNotFoundArray[i] = descriptionNotFound;
        //}
    }

    private void OnEnable()
    {
        SetDescriptionNotFoundText();

        underline = transform.Find("Underline").GetComponent<RectTransform>();

        SavedValues temp =
        GameController.instance.savedValuesInstance;
        bool exists;

        switch(collectedCheck)
        {
            case CollectedCheck.nothing:
                isFound = true;
                break;
            case CollectedCheck.itemID:
                exists = temp.collectables.TryGetValue(itemID, out isFound);
                if (!exists)
                    isFound = false;
                break;
            case CollectedCheck.levelCheck:
                exists = temp.levels.TryGetValue(levels[(int)itemLocation], out isFound);
                if (!exists)
                {
                    Debug.Log(levels[(int)itemLocation] + ": not found");
                    isFound = false;
                }
                break;
            case CollectedCheck.finalCutscene:
                exists = temp.finalCutsceneWatched;
                if (!exists)
                    isFound = false;
                break;
        }

        if(buttonText==null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (itemName.CompareTo("") == 0)
            itemName = buttonText.text;

        if (!isFound)
        {
            buttonText.text = "> " + nameNotFound;
            underline.sizeDelta = new Vector2(underlineLengthNotFound, underline.sizeDelta.y);
        }
        else if (collectedCheck != CollectedCheck.nothing)
        {
            buttonText.text = "> " + itemName;
            underline.sizeDelta = new Vector2(underlineLengthFound, underline.sizeDelta.y);
        }

        if(selectOnStart)
        SelectItem();
    }

    public void SelectItem()
    {
        if(isFound) //if found
        {
            headerText.text = itemName;
            descriptionText.text = itemDescription[0];

            if(itemImage != null)
                itemImage.color = colorFound;

            if(itemType == ItemType.Cutscene)
                cutsceneButton.SetActive(true);
        }
        else //if not found
        {
            headerText.text = nameNotFound;
            descriptionText.text = descriptionNotFound[0];

            if (itemImage != null)
                itemImage.color = colorNotFound;

            if (itemType == ItemType.Cutscene)
                cutsceneButton.SetActive(false);
        }

        //if (itemDescription.Length > 1)
            SetPage(0);

        // temp fix
        if (anim == null)
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
        if(isFound)
            multiPage.SetPage(value, itemDescription);
        else
            multiPage.SetPage(value, descriptionNotFound);

    }

    /// <summary>
    /// 
    /// </summary>
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
            case ItemType.Cutscene:
                text += "Unlock this cutscene by ";
                break;
        }

        if(itemType != ItemType.Cutscene)
        {
            switch (itemLocation)
            {
                case ItemLocation.Tutorial:
                    text += "in the Tunnels.";
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
        }

        else
        {
            switch(itemLocation)
            {
                case ItemLocation.Tutorial:
                    text += "rescuing Ezra.";
                    break;
                case ItemLocation.Town:
                    text += "investigating the Town";
                    break;
                case ItemLocation.Railyard:
                    text += "defeating the Overseer.";
                    break;
                case ItemLocation.Final:
                    text += "restoring peace to Grimstone Valley.";
                    break;
            }
        }

        descriptionNotFound[0] = text;
    }

    public void QueueCutscene()
    {
        //queue cutscene
    }

    public void PlayCutscene()
    {
        //play queued cutscene
    }
}