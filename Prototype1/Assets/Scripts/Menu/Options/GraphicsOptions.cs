using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraphicsOptions : MonoBehaviour
{
    [Header("Resolution Variables")]
    [SerializeField] [Tooltip("")] private TextMeshProUGUI resolutionText;
    [SerializeField] [Tooltip("")] private Resolution[] resolutions;

    private int currentResIndex = 0;

    [Header("Display Mode Variables")]
    [SerializeField] private TextMeshProUGUI displayModeText;

    [Header("VSync Variables")]
    [SerializeField] private TextMeshProUGUI vsyncText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fullscreen"></param>
    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;

        switch(fullscreen)
        {
            case true:
                displayModeText.text = "Fullscreen";
                PlayerPrefs.SetInt("DisplayMode", 1);
                break;
            case false:
                displayModeText.text = "Windowed";
                PlayerPrefs.SetInt("DisplayMode", 0);
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vsync"></param>
    public void SetVsync(bool vsync)
    {
        switch(vsync)
        {
            case true:
                QualitySettings.vSyncCount = 1;
                vsyncText.text = "On";
                PlayerPrefs.SetInt("VSync", 1);
                break;
            case false:
                QualitySettings.vSyncCount = 0;
                vsyncText.text = "Off";
                PlayerPrefs.SetInt("VSync", 0);
                break;
        }
    }
}
