using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SetVolume : MonoBehaviour
{
    [SerializeField] [Tooltip("")] private Slider slider;
    [SerializeField] [Tooltip("")] private TextMeshProUGUI volDisplay;

    [SerializeField] [Tooltip("Corresponding AudioMixer Group")] private AudioMixer mixer;
    [SerializeField] [Tooltip("Corrseponding exposed AudioMixer volume parameter name")] private string mixerVarName;

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
                SetLevel((float)val);
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
            newVal = 0.001f;
        else
            newVal = value / 20;

        mixer.SetFloat(mixerVarName, Mathf.Log10(newVal) * 20);
        PlayerPrefs.SetFloat(mixerVarName, value);

        volDisplay.text = (value * 5) + "%";
    }
}
