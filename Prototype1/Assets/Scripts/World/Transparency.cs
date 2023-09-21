using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparency : MonoBehaviour
{
    Material mat;
    [SerializeField]
    float fadeSpeed = 2.0f;
    [SerializeField]
    float fadeAmount = 0.4f;
    float originalOpacity;
    private bool fade = false;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        originalOpacity = mat.color.a;
        fade = false;
    }

    // Update is called once per frame
    void Update()
    {
        Color currentColor = mat.color;
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
                Mathf.Lerp(currentColor.a, originalOpacity, fadeSpeed * Time.deltaTime));
        }
        mat.color = smoothColor;
    }

    public void DoFade(bool fade)
    {
        Debug.Log("Setting fade to " + fade);
        this.fade = fade;
        Debug.Log("Fade is set to " + this.fade);
    }
}
