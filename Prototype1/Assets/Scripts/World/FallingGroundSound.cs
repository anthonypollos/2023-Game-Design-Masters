using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingGroundSound : MonoBehaviour
{
    
    AudioSource asource;
    
    // Start is called before the first frame update
    void Start()
    {
        asource = GetComponent<AudioSource>();
        //asource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound()
    {
        asource.Play();
    }

}
