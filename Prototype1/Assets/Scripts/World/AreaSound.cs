using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AreaSound : MonoBehaviour
{
    [SerializeField] VoiceClip clipToPlay;
    StudioEventEmitter emitter;
    BoxCollider collider;
    // Start is called before the first frame update
    void Start()
    {
        emitter = GetComponent<StudioEventEmitter>();
        if (emitter == null)
        {
            gameObject.SetActive(false);
        }
        collider = GetComponent<BoxCollider>();
        if(collider == null)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            collider.enabled = false;
            emitter.ChangeEvent(clipToPlay.eventReference);
            emitter.Play();
            SubtitleManager.instance.StartDialog(clipToPlay.subtitle, emitter);
        }
    }

}
