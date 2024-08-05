using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using FMODUnity;

public class CutsceneLoader : MonoBehaviour
{
    [SerializeField] private VideoClip[] cutscenes;
    [SerializeField] private VideoPlayer videoPlayer;

    [SerializeField] private StudioEventEmitter[] cutsceneEmitters;
    private StudioEventEmitter audioEmitter;

    [SerializeField] private GameObject skipPanel;
    [SerializeField] private Image skipProgressBar;
    [SerializeField] private Animator skipPanelAnim;

    [SerializeField] private VideoPlayer cutscene;

    [SerializeField] private Image startFade;
    private Color fade;
    private float fadeAlpha = 1;

    //[SerializeField] private GameObject audioManager;

    [SerializeField] private TextMeshProUGUI skipText;

    MainControls mainControls;

    private bool canSkip = true;
    private bool skipping = false;
    private bool triggeredAway = false;

    private int sceneToLoad = 0;

    public void SetClip(int index)
    {
        sceneToLoad = index;
        videoPlayer.clip = cutscenes[sceneToLoad];
        audioEmitter = cutsceneEmitters[sceneToLoad];
    }

    public void LoadClip()
    {
        audioEmitter.enabled = true;

        gameObject.SetActive(true);
        // set volume to 0 !
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        fade = startFade.color;
        fadeAlpha = 1;
        startFade.color = new Color(fade.r, fade.g, fade.b, fadeAlpha);

        mainControls = ControlsContainer.instance.mainControls;
        mainControls.Main.Interact.performed += ProgressBar;
        mainControls.Main.Interact.canceled += CancelProgress;

        MuteAudio(true);



        canSkip = true;
        skipping = false;
        triggeredAway = false;

        skipPanel.SetActive(false);

        string skipSprite = mainControls.Main.Interact.bindings[0].ToDisplayString().ToUpper().TranslateToSprite();
        skipText.text = "Hold<size=28>" + skipSprite + "</size>to Skip";

        cutscene.loopPointReached += Skip;
    }

    private void Update()
    {
        if (fadeAlpha > 0.1f)
        {
            fadeAlpha = Mathf.MoveTowards(fadeAlpha, 0f, 0.25f * Time.unscaledDeltaTime);

            startFade.color = new Color(fade.r, fade.g, fade.b, fadeAlpha);
        }
        else if (fadeAlpha != 0)
            fadeAlpha = 0;

        if (Input.anyKeyDown && skipPanel.activeInHierarchy == false)
        {
            skipPanel.SetActive(true);

            StartCoroutine(Away());
        }

        if (skipping)
        {
            triggeredAway = false;

            if (!skipPanel.activeInHierarchy)
                skipPanel.SetActive(true);

            float currentFill = skipProgressBar.fillAmount;
            skipProgressBar.fillAmount = Mathf.MoveTowards(currentFill, 1.1f, 1 * Time.unscaledDeltaTime);

            if (currentFill >= 1)
                Skip();
        }

        else if (!skipping)
        {
            skipProgressBar.fillAmount = 0;

            if (!triggeredAway && skipPanel.activeInHierarchy)
            {
                triggeredAway = true;
                //Invoke("Away", 2.0f);
                StopCoroutine(Away());
            }
        }
    }

    private void ProgressBar(InputAction.CallbackContext context)
    {
        if (canSkip)
        {
            skipping = true;
            StopCoroutine(Away());
        }
    }

    private void CancelProgress(InputAction.CallbackContext context)
    {
        if (canSkip)
            skipping = false;
    }

    // Update is called once per frame
    void Skip()
    {
        audioEmitter.enabled = false;

        StartCoroutine(Close());
    }

    private void Skip(UnityEngine.Video.VideoPlayer vid)
    {
        Skip();
    }

    IEnumerator Close()
    {
        yield return null;

        fadeAlpha = 1;
        startFade.color = new Color(fade.r, fade.g, fade.b, fadeAlpha);

        MuteAudio(false);
        gameObject.SetActive(false);
    }

    IEnumerator Away()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        //animation here
        skipPanel.SetActive(false);
    }

    private void MuteAudio(bool mute)
    {
        RuntimeManager.GetBus("bus:/SFX").setMute(mute);
        RuntimeManager.GetBus("bus:/Voice").setMute(mute);
        RuntimeManager.GetBus("bus:/Ambience").setMute(mute);
        RuntimeManager.GetBus("bus:/UI").setMute(mute);
        RuntimeManager.GetBus("bus:/Music").setMute(mute);
    }
}
