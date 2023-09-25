using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparency : MonoBehaviour
{
    Material[] mats;
    float fadeSpeed = 10f;
    [SerializeField]
    float fadeAmount = 0.4f;
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
            if (fade)
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
