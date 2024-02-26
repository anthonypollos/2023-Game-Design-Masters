/*
 * Avery
 */
using System.Collections;
using System.Collections.Generic;
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
    [Header("Audio Settings")]
    [SerializeField] [Tooltip("Volume Sliders")] private Slider[] volSliders;
    [SerializeField] [Tooltip("Exposed AudioMixer volume parameter names *IN SAME ORDER AS SLIDERS*")] private string[] mixerVarNames;

    [Header("---------------------------")]
    [Header("Default Setting Values")]
    [SerializeField] private int defaultFPS = 60;
    [SerializeField] [Range(0, 4)] private int defaultQuality = 1;
    [SerializeField] [Range(0, 20)] private float defaultBrightness = 10f;
    [SerializeField] [Range(0, 20)] private float defaultContrast = 10f;
    [SerializeField] [Range(0, 20)] private float defaultVolume = 15f;
    private int defaultResolutionIndex = 0;

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

        /* silly settings down here                                                                         */

        // 1 = sepia on, 0 = sepia off
        int sepia = PlayerPrefs.GetInt("SepiaMode", 0);
        switch (sepia)
        {
            case 1:
                graphicsOptions.SetSepia(true);
                break;
            case 0:
                graphicsOptions.SetSepia(false);
                break;
        }
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
            volSliders[i].value = PlayerPrefs.GetFloat(mixerVarNames[i], defaultVolume);
    }
    #endregion


    #region Reset to Defaults
    public void ResetVolume()
    {
        for (int i = 0; i < volSliders.Length; i++)
            volSliders[i].value = PlayerPrefs.GetFloat(mixerVarNames[i], defaultVolume);
    }

    public void ResetGraphics()
    {
        graphicsOptions.SetResolution(defaultResolutionIndex);
        graphicsOptions.SetFullscreen(true);
        graphicsOptions.SetVsync(false);
        graphicsOptions.SetFPS(defaultFPS);
        graphicsOptions.SetQuality(defaultQuality);
        graphicsOptions.SetBrightness(defaultBrightness);
        graphicsOptions.SetContrast(defaultContrast);
    }
    #endregion
}
