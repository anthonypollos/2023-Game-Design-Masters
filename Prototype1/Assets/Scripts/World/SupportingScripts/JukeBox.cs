using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class JukeBox
{
    [Serializable]
    public struct SoundTrack
    {
        public AudioClip sound;
        [Tooltip("The audio mixer group this sound should output to")] public AudioMixerGroup outputGroup;
        [Tooltip("On a scale of 0.0 to 1.0")] public float volume;
        [Tooltip("Does the sound play from the object or not")] public bool isLocalized;
        [Tooltip("How the AudioSource attenuates over distance")] public AudioRolloffMode rolloffMode;
        [Tooltip("The distance a sound starts attenuating at/becomes audible")] public float minDistance;
        [Tooltip("The distance a sound stops attenuating at/becomes inaudible")] public float maxDistance;
        public float spatialBlend;

    }

    [SerializeField] private SoundTrack[] tracks;
    [SerializeField] private Transform transform;

    public void SetTransform(Transform transform)
    {
        this.transform = transform;
    }


    [Tooltip("What audio clip to play. Input = index of sound clip")]
    public void PlaySound(int idx)
    {
 
        if(idx>tracks.Length-1 || tracks == null)
        {
            Debug.LogError("Error: That audio clip is not set");
            return;
        }
        else
        {
            SoundTrack track = tracks[idx];
            Vector3 location = track.isLocalized ? transform.position : Camera.main.transform.position;
            AudioSource.PlayClipAtPoint(track.sound, location, track.volume);
            //source.Play();
            Debug.Log("Should play");
        }
    }
}

