using UnityEngine;
using UnityEngine.UI;

public class SetOptions : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] [Tooltip("Mouse Sensitivity Slider")] private Slider mouseSensSlider;

    [Header("---------------------------")]
    [Header ("Volume")]
    [SerializeField] [Tooltip("Volume Sliders")] private Slider[] volSliders;
    [SerializeField] [Tooltip("Exposed AudioMixer volume parameter names *IN SAME ORDER AS SLIDERS*")] private string[] mixerVarNames;

    void Start()
    {
        SetPlayerPrefs();
    }

    /// <summary>
    /// Updates sliders to show PlayerPref values
    /// </summary>
    private void SetPlayerPrefs()
    {
        SetVolPrefs();
    }

    /// <summary>
    /// Updates volume sliders to show PlayerPref volume settings
    /// </summary>
    private void SetVolPrefs()
    {
        for (int i = 0; i < volSliders.Length; i++)
            volSliders[i].value = PlayerPrefs.GetFloat(mixerVarNames[i], 15f);
    }
}
