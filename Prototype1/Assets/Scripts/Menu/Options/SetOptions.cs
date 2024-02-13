using UnityEngine;
using UnityEngine.UI;

public class SetOptions : MonoBehaviour
{
    [Header("Control Settings")]
    [SerializeField] [Tooltip("Mouse Sensitivity Slider")] private Slider mouseSensSlider;

    [Header("---------------------------")]
    [Header("Graphics Settings")]
    [SerializeField] private GraphicsOptions graphicsOptions;

    [Header("---------------------------")]
    [Header ("Audio Settings")]
    [SerializeField] [Tooltip("Volume Sliders")] private Slider[] volSliders;
    [SerializeField] [Tooltip("Exposed AudioMixer volume parameter names *IN SAME ORDER AS SLIDERS*")] private string[] mixerVarNames;

    void Start()
    {
        //graphicsOptions = FindObjectOfType<GraphicsOptions>();

        SetPlayerPrefs();
        SetGraphicsSettings();
    }

    /// <summary>
    /// Updates sliders to show PlayerPref values
    /// </summary>
    private void SetPlayerPrefs()
    {
        SetVolPrefs();
    }

    /// <summary>
    /// 
    /// </summary>
    private void SetGraphicsSettings()
    {
        // 1 = fullscreen, 0 = windowed
        int displayMode = PlayerPrefs.GetInt("DisplayMode", 1);

        switch(displayMode)
        {
            case 1:
                graphicsOptions.SetFullscreen(true);
                break;
            case 0:
                graphicsOptions.SetFullscreen(false);
                break;
        }

        // 1 = vsync on, 0 = vsync off
        int vsync = PlayerPrefs.GetInt("VSync", 0);

        switch (vsync)
        {
            case 1:
                graphicsOptions.SetVsync(true);
                break;
            case 0:
                graphicsOptions.SetVsync(false);
                break;
        }
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
