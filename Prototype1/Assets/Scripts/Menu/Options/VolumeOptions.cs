using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class VolumeOptions : MonoBehaviour
{
    [SerializeField] [Tooltip("")] private Slider slider;
    [SerializeField] [Tooltip("")] private TextMeshProUGUI volDisplay;

    [SerializeField] [Tooltip("Corresponding AudioMixer Group")] private AudioMixer mixer;
    [SerializeField] [Tooltip("Corrseponding exposed AudioMixer volume parameter name")] private string mixerVarName;

    /// <summary>
    /// 
    /// </summary>
    private bool muted = false;

    /// <summary>
    /// 
    /// </summary>
    private float prevVal = 10;

    //temp
    public TextMeshProUGUI testMuteText;

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        Invoke("CheckSliderVal", 0.05f);
    }

    /// <summary>
    /// 
    /// </summary>
    private void CheckSliderVal()
    {
        if (slider.value == 0)
        {
            testMuteText.text = "muted";
            muted = true;
        }

        else
        {
            muted = false;
            testMuteText.text = "unmuted";
            prevVal = slider.value;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="mod"></param>
    public void AdjustVolume(int mod)
    {
        int val = (int)slider.value + mod;

        switch(val)
        {
            case (-1):
                // play Bad sound effect or "disable" button
                break;
            case (21):
                // play Bad sound effect or "disable" button
                break;
            default:
                slider.value = val;
                SetLevel(val);

                break;
        }
    }

    /// <summary>
    /// Adjusts AudioMixer volume based on slider input, updates volume PlayerPrefs
    /// </summary>
    /// <param name="value">Value set by slider</param>
    public void SetLevel(float value)
    {
        float newVal;

        if (value == 0)
        {
            if (!muted)
                ToggleMute();

            newVal = 0.001f;
        }
        else
        {
            prevVal = value;
            newVal = value / 20;

            if (muted)
                ToggleMute();
        }

        mixer.SetFloat(mixerVarName, Mathf.Log10(newVal) * 20);
        PlayerPrefs.SetFloat(mixerVarName, value);

        volDisplay.text = (value * 5) + "%";
    }

    /// <summary>
    /// 
    /// </summary>
    public void ToggleMute()
    {
        muted = !muted;

        switch(muted)
        {
            case (true):
                SetLevel(0);
                slider.value = 0;
                testMuteText.text = "muted";
                break;

            case (false):
                SetLevel(prevVal);
                slider.value = prevVal;
                testMuteText.text = "unmuted";
                break;
        }
    }
}
