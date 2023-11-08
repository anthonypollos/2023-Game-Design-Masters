using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

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
    private bool choiceNeeded;
    private bool choiceBuffer;
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
            mc.Main.Interact.Disable();
        }
    }

    private void Start()
    {
        choiceBuffer = false;
        choiceNeeded = false;
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
        dialogBox.gameObject.SetActive(false);
    }

    public void EnterDialogMode(TextAsset inkJSON)
    {
        Debug.Log("Starting Dialog");
        currentStory = new Story(inkJSON.text);
        Time.timeScale = 0f;
        dialogBox.SetActive(true);
        ContinueStory();


    }

    private void AttemptContinue()
    {
        if (storyStarted && !choiceNeeded)
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
                DisplayChoices();
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
        Debug.Log("Ending Dialog");
        yield return new WaitForSecondsRealtime(0.05f);
        foreach (GameObject button in choiceButtons)
            button.SetActive(false);
        dialogBox.SetActive(false);
        Time.timeScale = 1f;

    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;
        if(currentChoices.Count > choiceButtons.Count)
        {
            Debug.LogError("Not enough buttons in dialog manager");
        }
        int i = 0;
        foreach (Choice choice in currentChoices)
        {
            choiceButtons[i].SetActive(true);
            buttonText[i].text = choice.text;
            i++;
        }

        for (int temp = i; temp<choiceButtons.Count; temp++)
        {
            choiceButtons[temp].SetActive(false);
        }
        if (currentChoices.Count > 0)
        {
            topButton.Select();
            StartCoroutine(ChoiceBuffer());
            choiceNeeded = true;
        }
        else
        {
            choiceNeeded = false;
        }
        

    }

    private IEnumerator ChoiceBuffer()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        choiceBuffer = true;
    }

    public void Choose(int choice)
    {
        if (choiceBuffer)
        {
            choiceBuffer = false;
            currentStory.ChooseChoiceIndex(choice);
            choiceNeeded = false;
            ContinueStory();
        }
    }


}


