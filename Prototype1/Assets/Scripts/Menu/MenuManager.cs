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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetText();

        Invoke("ToggleText", textDelay);
    }

    private void Update()
    {
        // if input changes, update start text to show right prompt
        if (canContinue && (Input.anyKeyDown || Input.GetButtonDown("Submit")))
            anim.SetTrigger("NextState");

        LookAtButtons();
    }

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

    public void IntroEnd()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        canContinue = true;

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
}
