using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbianceTriggerChange : MonoBehaviour
{
    [SerializeField] private AmbianceArea ambiance;
    [SerializeField] private MusicArea bgmusic;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            print("player is here");
            AudioManager.instance.SetAmbianceArea(ambiance);
            AudioManager.instance.SetMusicArea(bgmusic);
        }
    }
}
