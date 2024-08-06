using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class UISound : MonoBehaviour
{
    //[SerializeField] private AudioSource audioSource;
    //[SerializeField] private AudioClip[] clip;

    //[SerializeField] private AudioManager audioManager;
    [SerializeField] private EventReference[] sounds;

    /// <summary>
    /// 
    /// </summary>
    public void PlaySound(int index)
    {
        //if (audioSource.isPlaying)
        //audioSource.Stop();
        //audioManager.

        AudioManager.instance.PlayOneShot(sounds[index], AudioManager.instance.transform.position);
    }
}
