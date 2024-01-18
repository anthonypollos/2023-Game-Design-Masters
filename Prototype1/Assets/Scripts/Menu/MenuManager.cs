using System.Collections;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    private InputChecker inputChecker;

    [SerializeField] [Tooltip("")] private GameObject startText;

    [SerializeField] [Tooltip("")] private string mouseText, controllerText;

    [SerializeField] [Tooltip("")] private float textDelay;

    private Animator anim;

    void Start()
    {
        inputChecker = GetComponent<InputChecker>();
        anim = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Invoke("EnableText", textDelay);
    }

    private void Update()
    {
        // if input changes, update start text to show right prompt
    }

    private void EnableText()
    {
        SetText();

        anim.SetTrigger("NextState");
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

        ParallaxObject[] parallaxObjects = FindObjectsOfType<ParallaxObject>();

        foreach (ParallaxObject po in parallaxObjects)
        {
            po.enabled = true;
        }
    }
}
