/*
 * Avery
 */
using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;

public class TestAudioLevel : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Reference to AudioSource component
    /// </summary>
    //private AudioSource audioSource;

    [SerializeField] private StudioEventEmitter eventEmitter;

    [SerializeField] private StudioEventEmitter[] emitters;

    //[SerializeField] [Tooltip("Audio clip to play after changing volume slider")] private AudioClip testClip;

    private void Start()
    {
        //audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Plays audio clip when mouse releases slider
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        //PlaySound();
        if (eventEmitter != null)
        {
            if (eventEmitter.IsPlaying())
                eventEmitter.Stop();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventEmitter.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        eventEmitter.enabled = false;
    }

    public void PlaySound()
    {
        foreach(StudioEventEmitter emitter in emitters)
        {
            emitter.Stop();
        }

        if(eventEmitter != null)
        {
            if (eventEmitter.IsPlaying())
                eventEmitter.Stop();

            eventEmitter.Play();
        }

        /*
        if (audioSource != null)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.PlayOneShot(testClip);
        }
        */
    }

}
