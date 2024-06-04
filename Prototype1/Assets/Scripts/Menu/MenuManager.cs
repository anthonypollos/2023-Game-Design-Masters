/*
 * Avery
 */
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

    void Start()
    {
        inputChecker = GetComponent<InputChecker>();
        anim = GetComponent<Animator>();

        StartCoroutine(IntroStart());
    }

    private void Update()
    {
        // if input changes, update start text to show right prompt
        SetText();

        if (Input.anyKeyDown)
            anim.SetTrigger("NextState");

        LookAtButtons();
    }

    /// <summary>
    /// 
    /// </summary>
    public void ToggleText(string toggle)
    {
        if (toggle == "off")
            anim.SetTrigger("TextOff");
        else if (toggle == "on")
            anim.SetTrigger("TextOn");
    }

    private void SetText()
    {
            startText.GetComponent<TextMeshProUGUI>().text = mouseText;
    }

    IEnumerator IntroStart()
    {
        SetText();

        yield return new WaitForSeconds(textDelay);

        ToggleText("on");
    }


    /// <summary>
    /// 
    /// </summary>
    public void IntroEnd()
    {
        ToggleParallax("on");
    }

    public void ToggleMainMenu()
    {
        mainMenuPanel.SetActive(!mainMenuPanel.activeSelf);
    }

    public void ToggleParallax(string toggle)
    {
        ParallaxObject[] parallaxObjects = FindObjectsOfType<ParallaxObject>();

        foreach (ParallaxObject po in parallaxObjects)
        {
            if (toggle == "on")
                po.enabled = true;
            else if (toggle == "off")
                po.enabled = false;
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
