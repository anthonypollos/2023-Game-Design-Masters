using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparency : MonoBehaviour
{
    Material[] mats;
    [SerializeField]
    float fadeSpeed = 2.0f;
    [SerializeField]
    float fadeAmount = 0.4f;
    float[] originalOpacities;
    private bool fade = false;

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
                //Debug.Log("Applying fade");
                smoothColor = new Color(currentColor.r, currentColor.g, currentColor.b,
                    Mathf.Lerp(currentColor.a, fadeAmount, fadeSpeed * Time.deltaTime));

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

    public void DoFade(bool fade)
    {
        Debug.Log("Setting fade to " + fade);
        this.fade = fade;
        Debug.Log("Fade is set to " + this.fade);
    }
}
