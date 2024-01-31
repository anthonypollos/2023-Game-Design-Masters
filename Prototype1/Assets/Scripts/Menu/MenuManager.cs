using System.Collections;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    private InputChecker inputChecker;

    [SerializeField] [Tooltip("")] private GameObject startText;

    [SerializeField] [Tooltip("")] private string mouseText, controllerText;

    [SerializeField] [Tooltip("")] private float textDelay;

    [SerializeField] [Tooltip("")] private GameObject mainMenuPanel;

    private Animator anim;

    private bool canContinue = false;

    void Start()
    {
        inputChecker = GetComponent<InputChecker>();
        anim = GetComponent<Animator>();

        // change this eventually; 0 false, 1 true
        int skip = PlayerPrefs.GetInt("SkipIntro", 0);

        switch (skip)
        {
            case 0:
                anim.SetBool("SkipIntro", false);

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                SetText();
                Invoke("ToggleText", textDelay);

                break;
            case 1:
                anim.SetBool("SkipIntro", true);
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        // if input changes, update start text to show right prompt
        if (canContinue && (Input.anyKeyDown || Input.GetButtonDown("Submit")))
            anim.SetTrigger("NextState");

        LookAtButtons();
    }

    /// <summary>
    /// 
    /// </summary>
    private void ToggleText()
    {
        anim.SetTrigger("ToggleText");
    }

    private void SetText()
    {
        if (inputChecker.IsController())
            startText.GetComponent<TextMeshProUGUI>().text = controllerText;

        else
            startText.GetComponent<TextMeshProUGUI>().text = mouseText;
    }

    /// <summary>
    /// 
    /// </summary>
    public void IntroEnd()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        canContinue = true;

        // change this
        PlayerPrefs.SetInt("SkipIntro", 1);

        ToggleParallax();
    }

    public void ToggleMainMenu()
    {
        mainMenuPanel.SetActive(!mainMenuPanel.activeSelf);
    }

    public void ToggleParallax()
    {
        ParallaxObject[] parallaxObjects = FindObjectsOfType<ParallaxObject>();

        foreach (ParallaxObject po in parallaxObjects)
        {
            po.enabled = !po.enabled;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private float lookIndex = 0;

    public void SetLookIndex(float lookIndex)
    {
        this.lookIndex = lookIndex;
    }

    private void LookAtButtons()
    {
        float currentIndex = anim.GetFloat("LookAtIndex");
        float targetIndex = Mathf.Lerp(currentIndex, lookIndex, 5.0f * Time.deltaTime);

        anim.SetFloat("LookAtIndex", targetIndex);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ResetIntroPref()
    {
        PlayerPrefs.SetInt("SkipIntro", 0);
    }

}
