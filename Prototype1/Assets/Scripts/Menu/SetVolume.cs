using UnityEngine;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour
{
    [SerializeField] [Tooltip("Corresponding AudioMixer Group")] private AudioMixer mixer;
    [SerializeField] [Tooltip("Corrseponding exposed AudioMixer volume parameter name")] private string mixerVarName;

    /// <summary>
    /// Adjusts AudioMixer volume based on slider input, updates volume PlayerPrefs
    /// </summary>
    /// <param name="sliderValue">Value set by slider</param>
    public void SetLevel(float sliderValue)
    {
        float newVal;

        if (sliderValue == 0)
            newVal = 0.001f;
        else
            newVal = sliderValue / 20;

        mixer.SetFloat(mixerVarName, Mathf.Log10(newVal) * 20);
        PlayerPrefs.SetFloat(mixerVarName, sliderValue);
    }
}
