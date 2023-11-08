using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance { get; private set; }
    [SerializeField] GameObject dialogBox;
    [SerializeField] TextMeshProUGUI mainText;
    [SerializeField] List<GameObject> choiceButtons;
    Button topButton;
    List<TextMeshProUGUI> buttonText;
    private Story currentStory;
    private bool storyStarted;
    MainControls mc;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one dialog manager");
            Destroy(this);
        }
        instance = this;
    }

    private void OnEnable()
    {
        if (mc == null)
        {
            mc = new MainControls();
        }
        mc.Main.Interact.Enable();
        mc.Main.Interact.performed += _ => AttemptContinue();
    }
    private void OnDisable()
    {
        if (mc != null)
        {

        }
    }

    private void Start()
    {
        currentStory = null;
        buttonText = new List<TextMeshProUGUI>();
        if (choiceButtons.Count != 0)
        {
            topButton = choiceButtons[0].GetComponent<Button>();
            int i = 0;
            foreach (GameObject temp in choiceButtons)
            {
                buttonText.Add(choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>());
                i++;
            }
        }
        gameObject.SetActive(false);
    }

    public void EnterDialogMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        Time.timeScale = 0f;
        dialogBox.SetActive(true);
        ContinueStory();


    }

    private void AttemptContinue()
    {
        if (storyStarted)
            ContinueStory();
    }

    private void ContinueStory()
    {
        if (currentStory != null)
        {
            if (currentStory.canContinue)
            {
                storyStarted = true;
                mainText.text = currentStory.Continue();
            }
            else
            {
                storyStarted = false;
                currentStory = null;
                mainText.text = "";
                StartCoroutine(EndStory());
            }
        }
    }

    private IEnumerator EndStory()
    {
        yield return new WaitForSecondsRealtime(0.05f);
        foreach (GameObject button in choiceButtons)
            button.SetActive(false);
        dialogBox.SetActive(false);
        Time.timeScale = 1f;

    }

    private void DisplayChoices()
    {

    }


}


