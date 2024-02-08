using UnityEngine;
using UnityEngine.EventSystems;

public class TestAudioLevel : MonoBehaviour, IPointerUpHandler
{
    /// <summary>
    /// Reference to AudioSource component
    /// </summary>
    private AudioSource audioSource;

    [SerializeField] [Tooltip("Audio clip to play after changing volume slider")] private AudioClip testClip;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Plays audio clip when mouse releases slider
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        PlaySound();
    }

    public void PlaySound()
    {
        if (audioSource != null)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.PlayOneShot(testClip);
        }
    }

}
