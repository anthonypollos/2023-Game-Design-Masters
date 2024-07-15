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



    public void StartDialog(string subText, StudioEventEmitter emitter)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        if (previousEmitter != null && previousEmitter!=emitter)
            previousEmitter.Stop();
        previousEmitter = emitter;
        coroutine = StartCoroutine(WaitTillFinish(subText, emitter));
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
