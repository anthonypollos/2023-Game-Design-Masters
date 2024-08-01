using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using FMOD;

public class CutsceneManager : MonoBehaviour
{
    public string sceneToLoad;

    [SerializeField] private GameObject skipPanel;
    [SerializeField] private Image skipProgressBar;
    [SerializeField] private Animator skipPanelAnim, transitionAnim;

    [SerializeField] private VideoPlayer cutscene;

    [SerializeField] private GameObject audioManager;

    MainControls mainControls;

    private bool canSkip = true;
    private bool skipping = false;
    private bool triggeredAway = false;

    [SerializeField] List<string> cutsceneScenes;

    // Start is called before the first frame update
    void OnEnable()
    {
        mainControls = ControlsContainer.instance.mainControls;
        mainControls.Main.Interact.performed += ProgressBar;
        mainControls.Main.Interact.canceled += CancelProgress;

        cutscene.loopPointReached += Skip;
        //cutscene.
    }

    private void Update()
    {
        if(skipping)
        {
            triggeredAway = false;

            if (!skipPanel.activeInHierarchy)
                skipPanel.SetActive(true);

            float currentFill = skipProgressBar.fillAmount;
            skipProgressBar.fillAmount = Mathf.MoveTowards(currentFill, 1.1f, 1 * Time.deltaTime);

            if (currentFill >= 1)
                Skip();
        }

        else if(!skipping)
        {
            skipProgressBar.fillAmount = 0;

            if (!triggeredAway && skipPanel.activeInHierarchy)
            {
                triggeredAway = true;
                Invoke("Away", 2.0f);
            }
        }
    }

    private void ProgressBar(InputAction.CallbackContext context)
    {
        if(canSkip)
        {
            skipping = true;
            CancelInvoke("Away");
        }
    }

    private void CancelProgress(InputAction.CallbackContext context)
    {
        if(canSkip)
            skipping = false;
    }

    // Update is called once per frame
    void Skip()
    {
        if(cutsceneScenes.Contains(SceneManager.GetActiveScene().name) && canSkip)
        {
            canSkip = false;
            transitionAnim.SetTrigger("Intro");
            audioManager.SetActive(false);
            Invoke("LoadNextScene", 1.0f);
        }
    }

    private void LoadNextScene()
    {
        SceneLoader.Instance.LoadScene(sceneToLoad);
    }

    private void Skip(UnityEngine.Video.VideoPlayer vid)
    {
        Skip();
    }

    private void Away()
    {
        //animation here
        skipPanel.SetActive(false);
    }
}
