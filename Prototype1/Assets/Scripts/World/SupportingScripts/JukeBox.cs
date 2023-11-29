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
        [Tooltip("Master audio mixer")] public AudioMixerGroup masterOutputGroup;   // can't figure out a way to get this via code unfortunately
        [Tooltip("The audio mixer group this sound should output to")] public AudioMixerGroup outputGroup;
        public enum mixerParameter { AmbientVol, SFXVol, MusicVol, VoiceVol }
        [Tooltip("The volume paramater of desired audio mixer group")] public mixerParameter outputVolParameter;

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
            //Debug.LogError("Error: That audio clip is not set");
            return;
        }
        else
        {
            SoundTrack track = tracks[idx];
            Vector3 location = track.isLocalized ? transform.position : Camera.main.transform.position;
            if (track.sound != null)
            {
                // get volume of desired output mixer
                bool value = track.outputGroup.audioMixer.GetFloat(track.outputVolParameter.ToString(), out float trackVol);

                // get volume of master volume mixer; NEED THIS to make sounds adjust with master volume
                bool masterValue = track.masterOutputGroup.audioMixer.GetFloat("MasterVol", out float masterVol);
                masterVol = Mathf.Pow(10f, masterVol / 20);

                // adjusts volume of clip; output mixer vol * track vol * master mixer vol
                float adjustVol = Mathf.Pow(10f, trackVol / 20) * track.volume * masterVol;

                AudioSource.PlayClipAtPoint(track.sound, location, adjustVol /*track.volume*/);
                //source.Play();
                //Debug.Log("Should play");
            }
            else
            {
                Debug.LogError("Error: That audio clip is not set");
            }
        }
    }
}

