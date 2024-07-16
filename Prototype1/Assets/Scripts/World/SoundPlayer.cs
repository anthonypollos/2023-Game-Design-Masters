///
/// Purpose: Play a oneshot sound from this.
/// Author: Sean Lee
/// Date: 7/16/24
///
///

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SoundPlayer : MonoBehaviour
{
    [Tooltip("The sound that gets played")]
    [SerializeField] private EventReference sound;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayOneShot(sound,transform.position);
    }
}
