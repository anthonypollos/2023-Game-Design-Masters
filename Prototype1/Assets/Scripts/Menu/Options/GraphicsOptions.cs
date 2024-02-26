using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GraphicsOptions : MonoBehaviour
{
    #region Variables
    [Header("Resolution Variables")]
    [SerializeField] [Tooltip("")] private TextMeshProUGUI resolutionText;
    [SerializeField] [Tooltip("")] public ResItem[] resolutions;
    private int currentResIndex = 0;
    private int defaultResIndex = 0;

    [Header("---------------------------")]
    [Header("Display Mode Variables")]
    [SerializeField] private TextMeshProUGUI displayModeText;
    [SerializeField] private string[] displayModeTexts;

    [Header("---------------------------")]
    [Header("VSync Variables")]
    [SerializeField] private TextMeshProUGUI vsyncText;
    [SerializeField] private string[] vsyncTexts;

    [Header("---------------------------")]
    [Header("Target FPS Variables")]
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private int[] targetFPS;
    private int currentFPSIndex = 0;

    [Header("---------------------------")]
    [Header("Graphics Quality Variables")]
    [SerializeField] private TextMeshProUGUI qualityText;
    [SerializeField] private string[] qualityTexts;
    private int currentQualityIndex = 0;
    private int defaultQualityIndex = 0;

    [Header("---------------------------")]
    [Header("Brightness & Contrast Variables")]
    [SerializeField] private TextMeshProUGUI[] brightnessTexts;
    [SerializeField] private TextMeshProUGUI[] contrastTexts;
    [SerializeField] private Slider[] brightnessSliders;
    [SerializeField] private Slider[] contrastSliders;
    [SerializeField] private VolumeProfile globalVolume;
    private ColorAdjustments colorAdjustments;

    [Header("---------------------------")]
    [Header("Funny Variables")]
    [SerializeField] private TextMeshProUGUI sepiaText;
    [SerializeField] private string[] sepiaTexts;
    [SerializeField] private ScriptableRendererFeature oldTimeyPass;
    private GameObject oldTimeyVol;
    private Volume vol;
    #endregion

    void Awake()
    {
        if(targetFPS.Length > 0)
            currentFPSIndex = System.Array.IndexOf(targetFPS, PlayerPrefs.GetInt("TargetFPS", 60));
    }

    #region Resolution
    /// <summary>
    /// Cycle through resolution options
    /// </summary>
    public void CycleResolution(int mod)
    {
        currentResIndex += mod;

        if (currentResIndex < 0)
            currentResIndex = 0;
        else if (currentResIndex >= resolutions.Length)
            currentResIndex = (resolutions.Length - 1);

        SetResolution(currentResIndex);
    }

    /// <summary>
    /// Set screen resolution to chosen setting
    /// </summary>
    /// <param name="index">Index of chosen resolution</param>
    public void SetResolution(int index)
    {
        currentResIndex = index;

        string ratio = resolutions[index].aspectRatio;
        int hor = resolutions[index].horizontal;
        int vert = resolutions[index].vertical;
        string defaultText = resolutions[index].defaultText;

        resolutionText.text = ratio + " (" + hor + " x " + vert + ") " + defaultText;

        PlayerPrefs.SetInt("Resolution", index);
        Screen.SetResolution(hor, vert, Screen.fullScreen);
    }

    /// <summary>
    /// Saves resolution closest to monitor's default size
    /// </summary>
    /// <param name="index">Index of default resolution</param>
    public void SetDefaultResolution(int index)
    {
        resolutions[index].defaultText = "(Default)";
        defaultResIndex = index;
    }
    #endregion

    #region Fullscreen
    /// <summary>
    /// Set display mode to fullscreen or windowed
    /// </summary>
    /// <param name="fullscreen">Is fullscreen?</param>
    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;

        switch(fullscreen)
        {
            //fullscreen on
            case true:
                displayModeText.text = displayModeTexts[1];
                PlayerPrefs.SetInt("DisplayMode", 1);
                break;
            //fullscreen off
            case false:
                displayModeText.text = displayModeTexts[0];
                PlayerPrefs.SetInt("DisplayMode", 0);
                break;
        }
    }
    #endregion

    #region VSync
    /// <summary>
    /// Enable or disable Vsync
    /// </summary>
    /// <param name="vsync">Is Vsync on?</param>
    public void SetVsync(bool vsync)
    {
        switch(vsync)
        {
            //vsync on
            case true:
                QualitySettings.vSyncCount = 1;
                vsyncText.text = vsyncTexts[1];
                PlayerPrefs.SetInt("VSync", 1);
                break;
            //vsync off
            case false:
                QualitySettings.vSyncCount = 0;
                vsyncText.text = vsyncTexts[0];
                PlayerPrefs.SetInt("VSync", 0);
                break;
        }
    }
    #endregion

    #region FPS
    /// <summary>
    /// Cycle through FPS options
    /// </summary>
    public void CycleFPS(int mod)
    {
        currentFPSIndex += mod;

        if (currentFPSIndex < 0)
            currentFPSIndex = 0;
        else if (currentFPSIndex >= targetFPS.Length)
            currentFPSIndex = (targetFPS.Length - 1);

        SetFPS(targetFPS[currentFPSIndex]);
    }

    /// <summary>
    /// Set target FPS to chosen setting
    /// </summary>
    /// <param name="framerate">Target FPS</param>
    public void SetFPS(int framerate)
    {
        fpsText.text = framerate + " FPS";

        Application.targetFrameRate = framerate;
        PlayerPrefs.SetInt("TargetFPS", framerate);
    }
    #endregion

    #region Graphics Quality
    /// <summary>
    /// Cycle through quality options
    /// </summary>
    public void CycleQuality(int mod)
    {
        currentQualityIndex += mod;

        if (currentQualityIndex < 0)
            currentQualityIndex = 0;
        else if (currentQualityIndex >= qualityTexts.Length)
            currentQualityIndex = (qualityTexts.Length - 1);

        SetQuality(currentQualityIndex);
    }

    /// <summary>
    /// Set graphics quality to chosen level
    /// </summary>
    /// <param name="index">Graphics quality level; 0 = low, 3 = ultra</param>
    public void SetQuality(int index)
    {
        qualityText.text = qualityTexts[index];

        PlayerPrefs.SetInt("TargetQuality", index);

        QualitySettings.SetQualityLevel(index);
    }
    
    #endregion

    #region Brightness
    /// <summary>
    /// Get target brightness value
    /// </summary>
    public void AdjustBrightness(int mod)
    {
        int val = (int)brightnessSliders[0].value + mod;

        switch (val)
        {
            case (-1):
                // play Bad sound effect or "disable" button
                break;
            case (21):
                // play Bad sound effect or "disable" button
                break;
            default:
                foreach(Slider slider in brightnessSliders)
                    slider.value = val;

                SetBrightness(val);
                break;
        }
    }

    /// <summary>
    /// Set brightness to target value
    /// </summary>
    /// <param name="value">Target brightness value</param>
    public void SetBrightness(float value)
    {
        PlayerPrefs.SetFloat("Brightness", value);

        foreach (TextMeshProUGUI text in brightnessTexts)
        {
            text.text = (value * 5) + "%";
        }

        foreach (Slider slider in brightnessSliders)
        {
            slider.value = value;
        }

        if (globalVolume.TryGet<ColorAdjustments>(out colorAdjustments))
            colorAdjustments.postExposure.value = (value / 13.3f);
    }
    #endregion

    #region Contrast
    /// <summary>
    /// Get target contrast value
    /// </summary>
    public void AdjustContrast(int mod)
    {
        int val = (int)contrastSliders[0].value + mod;

        switch (val)
        {
            case (-1):
                // play Bad sound effect or "disable" button
                break;
            case (21):
                // play Bad sound effect or "disable" button
                break;
            default:
                foreach (Slider slider in contrastSliders)
                    slider.value = val;

                SetContrast(val);
                break;
        }
    }

    /// <summary>
    /// Set target contrast value
    /// </summary>
    /// <param name="value">Target contrast value</param>
    public void SetContrast(float value)
    {
        PlayerPrefs.SetFloat("Contrast", value);

        foreach (TextMeshProUGUI text in contrastTexts)
        {
            text.text = (value * 5) + "%";
        }

        foreach (Slider slider in contrastSliders)
        {
            slider.value = value;
        }

        if (globalVolume.TryGet<ColorAdjustments>(out colorAdjustments))
            colorAdjustments.contrast.value = ((value - 10) * 2);
    }
    #endregion

    #region Sepia
    public void SetSepia(bool sepia)
    {
        if(oldTimeyVol == null)
            oldTimeyVol = GameObject.Find("Old Timey Volume");

        if(vol == null)
            vol = oldTimeyVol.GetComponent<Volume>();

        oldTimeyPass.SetActive(sepia);

        switch (sepia)
        {
            case true:
                sepiaText.text = sepiaTexts[1];
                PlayerPrefs.SetInt("SepiaMode", 1);
                vol.weight = 1;
                break;

            case false:
                sepiaText.text = sepiaTexts[0];
                PlayerPrefs.SetInt("SepiaMode", 0);
                vol.weight = 0;
                break;
        }
    }

    #endregion
}

[System.Serializable]
public class ResItem
{
    public string aspectRatio;
    public int horizontal, vertical;
    public string defaultText;
}