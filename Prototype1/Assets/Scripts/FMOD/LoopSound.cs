using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class LoopSound : MonoBehaviour
{
    [SerializeField] private EventReference loopingSound;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayOneShot(loopingSound, this.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
