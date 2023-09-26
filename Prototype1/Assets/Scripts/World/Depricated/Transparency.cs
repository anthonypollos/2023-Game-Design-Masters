using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparency : MonoBehaviour
{
    Material[] mats;
    float fadeSpeed = 10f;
    [SerializeField]
    float fadeAmount = 0.4f;
    [SerializeField]
    bool keepShadows = true;
    float[] originalOpacities;
    private bool fade = false;
    private bool lastFade = false;

    // Start is called before the first frame update
    void Start()
    {
        mats = GetComponent<Renderer>().materials;
        originalOpacities = new float[mats.Length];
        for (int i = 0; i < mats.Length; i++)
            originalOpacities[i] = mats[i].color.a;
        fade = false;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < mats.Length; i++)
        {
            Color currentColor = mats[i].color;
            Color smoothColor;
            if (fade && mats[i].HasProperty("_Color"))
            {
                if (lastFade)
                {
                    //Debug.Log("Applying fade");
                    smoothColor = new Color(currentColor.r, currentColor.g, currentColor.b,
                        Mathf.Lerp(currentColor.a, fadeAmount, fadeSpeed * Time.deltaTime));
                }
                else
                {
                    smoothColor = new Color(currentColor.r, currentColor.g, currentColor.b,
                        Mathf.Lerp(currentColor.a, 0, fadeSpeed * Time.deltaTime));
                }

            }
            else
            {
                //Debug.Log("Unapplying fade");
                smoothColor = new Color(currentColor.r, currentColor.g, currentColor.b,
                    Mathf.Lerp(currentColor.a, originalOpacities[i], fadeSpeed * Time.deltaTime));
                
            }
            mats[i].color = smoothColor;
            if(smoothColor.a >= originalOpacities[i])
            {
                mats[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mats[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mats[i].SetInt("_ZWrite", 1);
                mats[i].SetInt("_Surface", 0);

                mats[i].renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

                mats[i].SetShaderPassEnabled("DepthOnly", true);
                mats[i].SetShaderPassEnabled("SHADOWCASTER", true);

                mats[i].SetOverrideTag("RenderType", "Opaque");

                mats[i].DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mats[i].DisableKeyword("_ALPHAPREMULTIPLY_ON");
            }
            else
            {
                mats[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mats[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mats[i].SetInt("_ZWrite", 0);
                mats[i].SetInt("_Surface", 1);

                mats[i].renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                mats[i].SetShaderPassEnabled("DepthOnly", false);
                mats[i].SetShaderPassEnabled("SHADOWCASTER", keepShadows);

                mats[i].SetOverrideTag("RenderType", "Transparent");

                mats[i].EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mats[i].EnableKeyword("_ALPHAPREMULTIPLY_ON");
            }
        }
    }

    public void DoFade(bool fade, bool lastFade)
    {
        this.fade = fade;
        this.lastFade = lastFade;
        if(lastFade)
            Debug.Log("Last Fade set to " + name);
    }
}
