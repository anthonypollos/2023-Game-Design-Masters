using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FMODUnity;

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager instance { get; private set; }
    [SerializeField] TextMeshProUGUI textBox;
    StudioEventEmitter previousEmitter;
    Coroutine coroutine;
    [SerializeField] float lingerTime = 2f;
    [SerializeField] StudioEventEmitter defaultEmitter;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }
    private void Start()
    {
        textBox.gameObject.SetActive(false);
    }

    public void StartDialog(VoiceClip clip, bool waitTillUnpause = false)
    {
        if (!waitTillUnpause)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            if (previousEmitter != null && previousEmitter != defaultEmitter)
                previousEmitter.Stop();
            previousEmitter = defaultEmitter;
            defaultEmitter.ChangeEvent(clip.eventReference);
            defaultEmitter.Play();
            coroutine = StartCoroutine(WaitTillFinish(clip.subtitle, defaultEmitter));
        }
        else
        {
            StartCoroutine(WaitUntilUnpause(clip, defaultEmitter));
        }
            
    }


    public void StartDialog(string subText, StudioEventEmitter emitter)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        if (previousEmitter != null && previousEmitter!=emitter)
            previousEmitter.Stop();
        previousEmitter = emitter;
        coroutine = StartCoroutine(WaitTillFinish(subText, emitter));
    }

    public void CancleDialog()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        previousEmitter.Stop();
        textBox.gameObject.SetActive(false);
    }

    IEnumerator WaitUntilUnpause(VoiceClip clip, StudioEventEmitter emitter)
    {
        yield return new WaitForSecondsRealtime(1f);
        yield return new WaitUntil(() => Time.timeScale != 0);
        if (coroutine != null)
            StopCoroutine(coroutine);
        if (previousEmitter != null && previousEmitter != emitter)
            previousEmitter.Stop();
        previousEmitter = emitter;
        defaultEmitter.ChangeEvent(clip.eventReference);
        defaultEmitter.Play();
        coroutine = StartCoroutine(WaitTillFinish(clip.subtitle, emitter));
    }

    public IEnumerator WaitTillFinish(string subText, StudioEventEmitter emitter)
    {
        textBox.text = subText;
        textBox.gameObject.SetActive(true);
        yield return new WaitUntil(() => !emitter.IsPlaying());
        yield return new WaitForSecondsRealtime(lingerTime);
        textBox.gameObject.SetActive(false);
    }
}
