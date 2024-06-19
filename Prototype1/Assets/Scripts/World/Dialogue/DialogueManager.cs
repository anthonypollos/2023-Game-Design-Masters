using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using FMODUnity;

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
    private const string PLAY_VOICE_CLIP = "vc";
    private int lastVoiceClipValue;
    private string activeName;

    // Scrolling Text Variables
    private bool isScrolling, playDialogue;
    private Coroutine scrollText;
    [SerializeField] private float scrollTextDelay;

    private StudioEventEmitter studioEventEmitter;
    [SerializeField] private EventReference[] voiceClips;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one dialogue manager");
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
        studioEventEmitter = gameObject.GetComponent<StudioEventEmitter>();
        if (studioEventEmitter != null)
            studioEventEmitter.Stop();
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
        //Debug.Log(currentStory);
        //Debug.Log("Starting Dialog");
        pcNameText.text = "Maria";
        npcNameText.text = "";
        //imageAnimator.Play("Default");
        //portraitAnimator.Play("Default");
        currentStory = new Story(inkJSON.text);
        Time.timeScale = 0f;
        playerInteraction.Toggle();
        dialogBox.SetActive(true);
        //Debug.Log("enabling db");
        choiceBox.SetActive(false);
        backgroundPanel.SetActive(true);
        pcPortrait.SetActive(true);
        npcPortrait.SetActive(true);
        portraitAnimator.ResetTrigger("pcSpeak");
        portraitAnimator.ResetTrigger("npcSpeak");
        portraitAnimator.ResetTrigger("pcChoice");
        ContinueStory();


    }

    private void AttemptContinue()
    {
        //Debug.Log(storyStarted);
        if (storyStarted && !choiceNeeded)
            ContinueStory();
    }

    private string textToDisplay;

    private void ContinueStory()
    {
        if (currentStory != null)
        {
            // if text isn't scrolling, stop current audio
            if (studioEventEmitter!=null && !isScrolling)
            {
                studioEventEmitter.Stop();
            }
            if (currentStory.canContinue)
            {
                StartCoroutine(Buffer());
                //mainText.text = currentStory.Continue();

                // if story can be continued and text is not scrolling, start scrolling text for current line
                if (!isScrolling)
                {
                    playDialogue = true;
                    mainText.text = "";
                    textToDisplay = currentStory.Continue();
                    HandleTags();
                    DisplayChoices();
                    scrollText = StartCoroutine(ScrollText(textToDisplay));
                }
                // if story can be continued and text is scrolling, display current line's full text
                else if (isScrolling)
                {
                    StopCoroutine(scrollText);
                    mainText.maxVisibleCharacters = mainText.text.ToCharArray().Length;
                    isScrolling = false;
                }

                Debug.Log("enabling db");
                dialogBox.SetActive(true);    
            }

            else
            {
                // if on final line in current convo and text is scrolling, display final line's full text
                if(isScrolling)
                {
                    StopCoroutine(scrollText);
                    mainText.maxVisibleCharacters = mainText.text.ToCharArray().Length;
                    isScrolling = false;
                }
                // if on final line in current convo and text is fully displayed, end dialogue
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
                    npcPortrait.SetActive(true);
                    portraitAnimator.SetTrigger("npcSpeak");
                    portraitAnimator.ResetTrigger("pcSpeak");
                    portraitAnimator.ResetTrigger("pcChoice");
                    break;
                case NPC_PORTRAIT:
                    npcPortrait.SetActive(true);
                    float index = float.Parse(splitTags[1]);
                    portraitAnimator.SetFloat("npcIndex", index);
                    break;
                case NPC_NAME:
                    npcNameText.text = splitTags[1];
                    break;
                case PC_NAME:
                    pcNameText.text = splitTags[1];
                    break;
                case PLAY_VOICE_CLIP:
                    int index2 = int.Parse(splitTags[1]);
                    lastVoiceClipValue = index2;
                    if (studioEventEmitter != null && playDialogue) //prevents dialogue lines from playing twice when skipping scrolling text
                    {
                        studioEventEmitter.Stop();
                        if (index2 < voiceClips.Length)
                        {
                            studioEventEmitter.ChangeEvent(voiceClips[index2]);
                            studioEventEmitter.Play();
                            playDialogue = false;
                        }
                        else
                        {
                            Debug.LogError("Index doesn't match for voice clips");
                        }
                    }
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
        currentStory = null;
        playerInteraction.Toggle();
        //Debug.Log("Ending Dialog");
        yield return new WaitForSecondsRealtime(0.05f);
        foreach (GameObject button in choiceButtons)
            button.SetActive(false);
        portraitAnimator.ResetTrigger("pcSpeak");
        portraitAnimator.ResetTrigger("pcChoice");
        portraitAnimator.ResetTrigger("npcSpeak");
        portraitAnimator.Play("Empty");
        dialogBox.SetActive(false);
        //Debug.Log("disabling db");
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
            //topButton.Select();
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
            portraitAnimator.SetTrigger("pcSpeak");
            portraitAnimator.ResetTrigger("npcSpeak");
            portraitAnimator.ResetTrigger("pcChoice");
            ContinueStory();
        }
    }

    private IEnumerator ScrollText(string text)
    {
        isScrolling = true;

        int maxVisibleChars = 0;

        mainText.text = text;
        mainText.maxVisibleCharacters = maxVisibleChars;

        foreach (char c in text.ToCharArray())
        {
            maxVisibleChars++;
            mainText.maxVisibleCharacters = maxVisibleChars;

            yield return new WaitForSecondsRealtime(scrollTextDelay);
        }

        isScrolling = false;
    }
}


