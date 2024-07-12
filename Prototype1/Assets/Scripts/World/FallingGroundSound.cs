using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FallingGroundSound : MonoBehaviour
{

    [SerializeField] private EventReference groundFall;

    // Start is called before the first frame update
    void Start()
    {
        
        //asource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound()
    {
        AudioManager.instance.PlayOneShot(groundFall, this.transform.position);
    }

}
