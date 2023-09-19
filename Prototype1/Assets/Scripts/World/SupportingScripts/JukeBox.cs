using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JukeBox
{
    [Serializable]
    public struct SoundTrack
    {
        public AudioClip sound;
        public float volume;
        [Tooltip("Does the sound play from the object or not")] 
        public bool isLocalized;

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
        if(idx>tracks.Length-1 || tracks[idx].sound == null)
        {
            Debug.LogError("Error: That audio clip is not set");
            return;
        }
        else
        {
            SoundTrack track = tracks[idx];
            Vector3 location = track.isLocalized ? transform.position : Camera.main.transform.position;
            AudioSource.PlayClipAtPoint(track.sound, location, track.volume);
        }
    }
}

