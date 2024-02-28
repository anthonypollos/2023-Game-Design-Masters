using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clip;

    /// <summary>
    /// 
    /// </summary>
    public void PlaySound(int index)
    {
        if (audioSource.isPlaying)
            audioSource.Stop();

        audioSource.PlayOneShot(clip[index]);
    }
}
