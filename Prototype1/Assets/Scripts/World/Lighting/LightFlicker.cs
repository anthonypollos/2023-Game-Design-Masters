using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose: Adds a slight noisy flicker to whichever realtime light this script is placed on
/// Author: Sean Lee (1/24/24)
/// </summary>


public class LightFlicker : MonoBehaviour
{

    private float flickerScale = 1f;
    [Tooltip("The lowest intensity the flicker can go to. Default is 0.8.\nMultiplied by flickerScale.")]
    public float flickerMin = 0.9f;
    [Tooltip("The highest intensity the flicker can go to. Default is 1.2.\nMultiplied by flickerScale.")]
    public float flickerMax = 1.1f;
    [Tooltip("The number of times this light changes per second.")]
    public float rate = 30f;
    [Tooltip("DEBUG:\nPrint verbose information about this light flicker to the console.")]
    [SerializeField] protected bool debugVerbose = false;

    private bool isFlickering = false;

    //The component of the light we're going to be flickering
    private Light Lightcomponent;
    //The light's original intensity;
    private float LightIntensity;

    // Start is called before the first frame update
    void Start()
    {
        Lightcomponent = GetComponent<Light>();
        LightIntensity = GetComponent<Light>().intensity;

        //Ideally, we want InvokeRepeating to cancel if the light is inactive and then re-enable when the light becomes active
        StartFlicker();
    }

    public void StartFlicker()
    {
        if (Lightcomponent != null && !isFlickering)
        {
            if (debugVerbose) print("StartFlicker called on " + gameObject + "\nRate is " + rate + " flickers per second.");
            InvokeRepeating(nameof(Flicker), Random.Range(0, 3f), 1f / rate);
            isFlickering = true;
        }
    }

    public void StopFlicker()
    {
        CancelInvoke("Flicker");
        isFlickering = false;
    }

    void Flicker()
    {
       Lightcomponent.intensity = LightIntensity * (Random.Range(flickerMin, flickerMax) * flickerScale);
        if (debugVerbose) print(gameObject + " intensity: " + Lightcomponent.intensity);
    }
}
