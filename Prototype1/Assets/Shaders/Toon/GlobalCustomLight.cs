using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GlobalCustomLight : MonoBehaviour
{
    [Header("FOR TESTING IN EDITOR")]
    public bool inEditor = false;

    public Material[] customLitMats;

    [ColorUsageAttribute(true, true)]
    public Color globalLightColor;

    [Tooltip("Keep values between -1 and 1 for best results")]
    [Range(-1,1)]
    public float globalLightDirX, globalLightDirY, globalLightDirZ;

    // Start is called before the first frame update
    void Start()
    {
        UpdateLighting();
    }

    // Update is called once per frame
    void Update()
    {
        if (inEditor)
            UpdateLighting();
    }

    private void UpdateLighting()
    {
        foreach (Material mat in customLitMats)
        {
            //mat.SetFloat("_LightDirX", globalLightDirX);
            //mat.SetFloat("_LightDirY", globalLightDirY);
            //mat.SetFloat("_LightDirZ", globalLightDirZ);

            mat.SetColor("_LightColor", globalLightColor);
        }
    }
}
