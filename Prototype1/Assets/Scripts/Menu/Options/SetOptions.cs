/*
 * Avery
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetOptions : MonoBehaviour
{
    [Header("---------------------------")]
    [Header("Graphics Settings")]
    [SerializeField] private GraphicsOptions graphicsOptions;

    [Header("---------------------------")]
    [Header("Audio Settings")]
    [SerializeField] [Tooltip("Volume Sliders")] private Slider[] volSliders;
    [SerializeField] [Tooltip("Volume Panels *IN SAME ORDER AS SLIDERS*")] private VolumeOptions[] volOptions;
    [SerializeField] [Tooltip("Exposed AudioMixer volume parameter names *IN SAME ORDER AS SLIDERS*")] private string[] mixerVarNames;

    [Header("---------------------------")]
    [Header("Accessibility Settings")]
    [SerializeField] private AccessibilityOptions accessOptions;

    [Header("---------------------------")]
    [Header("Default Setting Values")]
    [SerializeField] private int defaultFPS = 60;
    [SerializeField] [Range(0, 4)] private int defaultQuality = 1;
    [SerializeField] [Range(0, 20)] private float defaultBrightness = 6f;
    [SerializeField] [Range(0, 20)] private float defaultContrast = 6f;
    [SerializeField] [Range(0, 20)] private float defaultVolume = 15f;
    private int defaultResolutionIndex = 0;

    private float defaultCursorScale = 12f;
    [SerializeField] private Color defaultOuterCursorColor;
    [SerializeField] private Color defaultInnerCursorColor;

    private float defaultOutlineWidth = 17f;
    [SerializeField] private Color defaultOutlineColor;

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
        SetGraphicsSettings();
        SetAccessibiltyOptions();
    }

    #region Graphics Functions
    /// <summary>
    /// 
    /// </summary>
    private void SetGraphicsSettings()
    {
        // get screen's default aspect ratio,
        SetResolution();

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

        graphicsOptions.SetFPS(PlayerPrefs.GetInt("TargetFPS", defaultFPS));

        graphicsOptions.SetQuality(PlayerPrefs.GetInt("TargetQuality", defaultQuality));

        graphicsOptions.SetBrightness(PlayerPrefs.GetFloat("Brightness", defaultBrightness));
        graphicsOptions.SetContrast(PlayerPrefs.GetFloat("Contrast", defaultContrast));
    }

    #region Resolution Functions
    /// <summary>
    /// 
    /// </summary>
    private void SetResolution()
    {
        List<int> aspect16_10 = new List<int>();
        List< int > aspect16_9 = new List<int>();
        List<int> aspect21_9 = new List<int>();

        foreach (ResItem item in graphicsOptions.resolutions)
        {
            switch(item.aspectRatio)
            {
                case "16:10":
                    aspect16_10.Add(item.horizontal);
                    break;
                case "16:9":
                    aspect16_9.Add(item.horizontal);
                    break;
                case "21:9":
                    aspect21_9.Add(item.horizontal);
                    break;
            }
        }

        float width = Screen.width;
        float height = Screen.height;

        float ratio = width / height;

        int closestResIndex;

        if (ratio < 1.7)
            closestResIndex = FindClosest(width, "16:10", aspect16_10);
        else if (ratio > 2.2)
            closestResIndex = FindClosest(width, "21:9", aspect21_9);
        else
            closestResIndex = FindClosest(width, "16:9", aspect16_9);

        graphicsOptions.SetDefaultResolution(closestResIndex);

        defaultResolutionIndex = closestResIndex;
        int resIndex = PlayerPrefs.GetInt("Resolution", defaultResolutionIndex);
        graphicsOptions.SetResolution(resIndex);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="width"></param>
    /// <param name="ratioList"></param>
    private int FindClosest(float width, string aspect, List<int> ratioList)
    {
        float lastDiff, difference;
        int closestIndex = 0;

        lastDiff = Mathf.Abs(width - ratioList[0]);

        for(int i = 1; i < ratioList.Count; i++)
        {
            difference = Mathf.Abs(width - ratioList[i]);

            if (difference < lastDiff)
                closestIndex = i;

            lastDiff = difference;
        }

        int closestWidth = ratioList[closestIndex];

        int index = 0;

        for(int i = 0; i < graphicsOptions.resolutions.Length; i++)
        {
            if (graphicsOptions.resolutions[i].horizontal == closestWidth &&
                graphicsOptions.resolutions[i].aspectRatio == aspect)
            {
                index = i;
                break;
            }
        }
        return index;
    }
    #endregion
    #endregion

    #region Volume Functions
    /// <summary>
    /// Updates volume sliders to show PlayerPref volume settings
    /// </summary>
    private void SetVolPrefs()
    {
        for (int i = 0; i < volSliders.Length; i++)
        {
            float vol = PlayerPrefs.GetFloat(mixerVarNames[i], defaultVolume);

            volSliders[i].value = vol;
            volOptions[i].SetBusVolume(vol/20);
        }
    }
    #endregion

    #region Accessibilty Functions
    /// <summary>
    /// 
    /// </summary>
    private void SetAccessibiltyOptions()
    {
        accessOptions.SetCursorScale(PlayerPrefs.GetFloat("CursorScale", defaultCursorScale));

        float innerColorR = PlayerPrefs.GetFloat("InnerCursorR", defaultInnerCursorColor.r);
        float innerColorG = PlayerPrefs.GetFloat("InnerCursorG", defaultInnerCursorColor.g);
        float innerColorB = PlayerPrefs.GetFloat("InnerCursorB", defaultInnerCursorColor.b);

        Color innerColor = new Color(innerColorR, innerColorG, innerColorB);

        float outerColorR = PlayerPrefs.GetFloat("OuterCursorR", defaultOuterCursorColor.r);
        float outerColorG = PlayerPrefs.GetFloat("OuterCursorG", defaultOuterCursorColor.g);
        float outerColorB = PlayerPrefs.GetFloat("OuterCursorB", defaultOuterCursorColor.b);

        Color outerColor = new Color(outerColorR, outerColorG, outerColorB);

        accessOptions.SetCursorColor(innerColor, outerColor);

        // set outline width
        accessOptions.SetOutlineWidth(PlayerPrefs.GetFloat("OutlineWidth", defaultOutlineWidth));

        // set outline colors
        accessOptions.SetOutlineColors();
        accessOptions.UpdateOutlineColors();

    }
    #endregion

    #region Reset to Defaults
    public void ResetVolume()
    {
        for (int i = 0; i < volSliders.Length; i++)
            volSliders[i].value = defaultVolume;
    }

    public void ResetGraphics()
    {
        graphicsOptions.SetResolution(defaultResolutionIndex);
        graphicsOptions.SetFullscreen(true);
        graphicsOptions.SetVsync(false);
        graphicsOptions.SetFPS(defaultFPS);
        graphicsOptions.SetQuality(defaultQuality);
        //ResetBrightnessContrast();
        //graphicsOptions.SetSepia(false);
    }

    public void ResetBrightnessContrast()
    {
        graphicsOptions.SetBrightness(defaultBrightness);
        graphicsOptions.SetContrast(defaultContrast);
    }

    public void ResetAccessibility()
    {
        accessOptions.SetCursorScale(defaultCursorScale);

        accessOptions.SetOutlineWidth(defaultOutlineWidth);
    }

    public void ResetCursorColor()
    {
        accessOptions.SetCursorColor(defaultInnerCursorColor, defaultOuterCursorColor);
    }

    public void ResetOutlineColor()
    {
        accessOptions.ResetSliders();
    }
    #endregion
}
