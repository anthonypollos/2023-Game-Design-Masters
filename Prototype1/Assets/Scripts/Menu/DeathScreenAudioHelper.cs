using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class DeathScreenAudioHelper : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke("MuteAudio", 1f);
    }

    private void OnDisable()
    {
        RuntimeManager.GetBus("bus:/SFX").setMute(false);
    }

    private void MuteAudio()
    {
        RuntimeManager.GetBus("bus:/SFX").setMute(true);
    }
}
