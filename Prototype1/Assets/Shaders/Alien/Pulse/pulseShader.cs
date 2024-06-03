using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pulseShader : MonoBehaviour
{
    [Header("FOR TESTING IN EDITOR")]
    public bool inEditor = false;

    private Material pulseMat;

    [Header("------------------")]
    [Header("Pulse Properties")]
    [SerializeField] [Tooltip("Toggle scrolling pulse effect")]
    private bool scrollPulse;

    [SerializeField] [Range(0,5)] [Tooltip("Speed of scrolling pulse")]
    private float pulseSpeed = 1f;

    [SerializeField] [Range(0, 10)] [Tooltip("Strength of pulse effect mesh displacement")]
    private float pulseStrength = 1f;

    [SerializeField] [Tooltip("INVERTED! Larger number = smaller width")] [Range(0, 10)]
    private float pulseWidth = 0.5f;


    [SerializeField] [Tooltip("Where should panning start/stop? Adjust numbers if loop isn't clean")]
    private float panStart = -0.5f, panStop = 2f;

    private float pulsePan = 0.5f;

    [Header("------------------")]
    [Header("Noise Properties")]
    [SerializeField] [Range(0,5)] [Tooltip("Strength of noise effect mesh displacement")]
    private float noiseStrength = 1f;

    [SerializeField] [Range(0,3)] [Tooltip("Speed of noise effect/movement")]
    private float noiseSpeed = 1f;

    [SerializeField] [Range(0,15)] [Tooltip("Amount/density of noise")]
    private float noiseIntensity = 1f;

    [Header("------------------")]
    [Header("UV Properties - For Scroll Pulse Effect")]
    [SerializeField] [Tooltip("Scroll through UV vertically?")] private bool scrollVertical;
    [SerializeField] [Tooltip("Invert UVs Vertically?")]private bool invertVertical;
    [SerializeField] [Tooltip("Invert UVs Horizontally?")] private bool invertHorizontal;

    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<MeshRenderer>() != null)
            pulseMat = GetComponent<MeshRenderer>().material;
        else if(GetComponent<SkinnedMeshRenderer>() != null)
            pulseMat = GetComponent<SkinnedMeshRenderer>().material;

        AssignVariables();

        //pulseMat = GetComponent<MeshRenderer>().material;

        if (scrollPulse)
            pulsePan = panStart;
    }

    // Update is called once per frame
    void Update()
    {
        if(inEditor)
            AssignVariables();

        if (scrollPulse)
        {
            if (pulsePan > panStop)
                pulsePan = panStart;
            else if (pulsePan < panStop)
                pulsePan = Mathf.Lerp(pulsePan, panStop + 1, (pulseSpeed/10) * Time.deltaTime);

            pulseMat.SetFloat("_Pan", pulsePan);
        }
    }

    private void AssignVariables()
    {
        if (scrollPulse)
        {
            pulseMat.SetFloat("_ScrollPulse", 1);
            //pulsePan = panStart;
        }
        else
        {
            pulseMat.SetFloat("_ScrollPulse", 0);
            pulseMat.SetFloat("_Pan", 0.5f);
            pulseWidth = 0;
            pulseStrength = 1;
        }

        if (scrollVertical)
            pulseMat.SetFloat("_ScrollVertical", 1);
        else
            pulseMat.SetFloat("_ScrollVertical", 0);

        if (invertVertical)
            pulseMat.SetFloat("_InvertUVVertical", 1);
        else
            pulseMat.SetFloat("_InvertUVVertical", 0);

        if (invertHorizontal)
            pulseMat.SetFloat("_InvertUVHorizontal", 1);
        else
            pulseMat.SetFloat("_InvertUVHorizontal", 0);


        pulseMat.SetFloat("_PulseStrength", pulseStrength);
        pulseMat.SetFloat("_PulseWidth", pulseWidth);


        pulseMat.SetFloat("_NoiseStrength", noiseStrength / 10);
        pulseMat.SetFloat("_NoiseSpeed", noiseSpeed / 10);
        pulseMat.SetFloat("_NoiseIntensity", noiseIntensity);
    }
}
