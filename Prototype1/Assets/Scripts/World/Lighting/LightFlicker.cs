using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose: Adds a slight noisy flicker to whichever realtime light this script is placed on
/// Author: Sean Lee (1/24/24)
/// </summary>


public class LightFlicker : MonoBehaviour
{
    //Privated because I've realized that this scale factor won't work at the moment
    //[Tooltip("The Strength of the flickering. Default is 1.")]
    private float flickerScale = 1f;
    [Tooltip("The lowest intensity the flicker can go to. Default is 0.8.\nMultiplied by flickerScale.")]
    public float flickerMin = 0.9f;
    [Tooltip("The highest intensity the flicker can go to. Default is 1.2.\nMultiplied by flickerScale.")]
    public float flickerMax = 1.1f;
    [Tooltip("The number of times this light changes per second.")]
    public float rate = 30f;
    //The component of the light we're going to be flickering
    private Light Lightcomponent;
    //The light's original intensity;
    private float LightIntensity;

    // Start is called before the first frame update
    void Start()
    {
        Lightcomponent = GetComponent<Light>();
        LightIntensity = GetComponent<Light>().intensity;
        InvokeRepeating("Flicker", Random.Range(0, 3f), (1f / rate));
    }

    // FixedUpdate so that the flickering isn't too fast. Also improves perf
    void FixedUpdate()
    {
        //Lightcomponent.intensity = LightIntensity * ( Random.Range(flickerMin,flickerMax) * flickerScale );
    }

    void Flicker()
    {
        Lightcomponent.intensity = LightIntensity * (Random.Range(flickerMin, flickerMax) * flickerScale);
    }
}
