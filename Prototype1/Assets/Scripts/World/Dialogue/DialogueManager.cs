using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance { get; private set; }
    [SerializeField] GameObject dialogBox;
    [SerializeField] GameObject choiceBox;
    [SerializeField] GameObject backgroundPanel;
    [SerializeField] GameObject pcPortrait;
    [SerializeField] GameObject npcPortrait;
    [SerializeField] TextMeshProUGUI mainText;
    [SerializeField] List<GameObject> choiceButtons;
    Button topButton;
    List<TextMeshProUGUI> buttonText;
    private Story currentStory;
    private bool storyStarted;
    private bool choiceNeeded;
    private bool choiceBuffer;
    MainControls mc;
    InteractBehavior playerInteraction;

    [SerializeField] TextMeshProUGUI pcNameText;
    [SerializeField] TextMeshProUGUI npcNameText;
    //[SerializeField] Animator imageAnimator;
    [SerializeField] Animator portraitAnimator;
    
    private const string IMAGE_TAG = "i";
    private const string PC_PORTRAIT = "pp";
    private const string NPC_PORTRAIT = "np";
    private const string PC_SPEAKER = "ps";
    private const string NPC_SPEAKER = "ns";
    private const string PC_NAME = "pn";
    private const string NPC_NAME = "nn";
    private string activeName;

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
        mc = ControlsContainer.instance.mainControls;
        mc.Main.Interact.performed += Interact;
    }
    private void OnDisable()
    {
        mc.Main.Interact.performed -= Interact;
    }

    private void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            AttemptContinue();
    }

    private void Start()
    {
        choiceBuffer = false;
        choiceNeeded = false;
        currentStory = null;
        pcNameText.text = "Maria";
        npcNameText.text = "";
        //imageAnimator.Play("Default");
        //portraitAnimator.Play("Default");
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
        Debug.Log("disabling db");
    }

    public void SetPlayerInteraction(InteractBehavior temp)
    {
        //Debug.Log(temp.name);
        playerInteraction = temp;
    }
    public void EnterDialogMode(TextAsset inkJSON)
    {
        Debug.Log(currentStory);
        Debug.Log("Starting Dialog");
        pcNameText.text = "Maria";
        npcNameText.text = "";
        //imageAnimator.Play("Default");
        //portraitAnimator.Play("Default");
        currentStory = new Story(inkJSON.text);
        Time.timeScale = 0f;
        playerInteraction.Toggle();
        dialogBox.SetActive(true);
        Debug.Log("enabling db");
        choiceBox.SetActive(false);
        backgroundPanel.SetActive(true);
        pcPortrait.SetActive(true);
        npcPortrait.SetActive(true);
        ContinueStory();


    }

    private void AttemptContinue()
    {
        Debug.Log(storyStarted);
        if (storyStarted && !choiceNeeded)
            ContinueStory();
    }

    private void ContinueStory()
    {
        if (currentStory != null)
        {
            if (currentStory.canContinue)
            {
                StartCoroutine(Buffer());
                mainText.text = currentStory.Continue();
                HandleTags();
                DisplayChoices();
            }
            else
            {
                //Debug.Log("Set to false");
                storyStarted = false;
                currentStory = null;
                mainText.text = "";
                StartCoroutine(EndStory());
            }
        }
    }

    IEnumerator Buffer()
    {
        //Debug.Log("Coroutine Started");
        yield return new WaitForSecondsRealtime(0.1f);
        storyStarted = true;
        //Debug.Log("Set to true");
    }

    private void HandleTags()
    {
        List<string> tags = currentStory.currentTags;
        foreach (string tag in tags)
        {
            string[] splitTags = tag.Split(":");
            Debug.Log(tag);

            switch(splitTags[0])
            {
                /*
                case IMAGE_TAG:
                    imageAnimator.Play(splitTags[1]);
                    break;
                */
                case PC_SPEAKER:
                    portraitAnimator.SetTrigger("pcSpeak");
                    portraitAnimator.ResetTrigger("npcSpeak");
                    portraitAnimator.ResetTrigger("pcChoice");
                    break;
                case NPC_SPEAKER:
                    portraitAnimator.SetTrigger("npcSpeak");
                    portraitAnimator.ResetTrigger("pcSpeak");
                    portraitAnimator.ResetTrigger("pcChoice");
                    break;
                case NPC_PORTRAIT:
                    float index = float.Parse(splitTags[1]);
                    portraitAnimator.SetFloat("npcIndex", index);
                    break;
                case NPC_NAME:
                    npcNameText.text = splitTags[1];
                    break;
                case PC_NAME:
                    pcNameText.text = splitTags[1];
                    break;
                default: 
                    Debug.LogError("Tag not recognized");
                    Debug.LogError(tag);
                    break;
            }
        }
    }

    private IEnumerator EndStory()
    {
        playerInteraction.Toggle();
        Debug.Log("Ending Dialog");
        yield return new WaitForSecondsRealtime(0.05f);
        foreach (GameObject button in choiceButtons)
            button.SetActive(false);
        portraitAnimator.Play("Empty");
        dialogBox.SetActive(false);
        Debug.Log("disabling db");
        choiceBox.SetActive(false);
        backgroundPanel.SetActive(false);
        pcPortrait.SetActive(false);
        npcPortrait.SetActive(false);
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
            portraitAnimator.SetTrigger("pcChoice");
            portraitAnimator.ResetTrigger("pcSpeak");
            portraitAnimator.ResetTrigger("npcSpeak");
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
            portraitAnimator.Play("dialogue_PCspeak");
            ContinueStory();
        }
    }


}


