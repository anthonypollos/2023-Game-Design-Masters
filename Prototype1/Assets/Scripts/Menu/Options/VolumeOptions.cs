/*
 * Avery
 */
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using FMODUnity;
using FMOD.Studio;

public class VolumeOptions : MonoBehaviour
{
    [SerializeField] [Tooltip("")] private Slider slider;
    [SerializeField] [Tooltip("")] private TextMeshProUGUI volDisplay;

    [SerializeField] [Tooltip("Corresponding AudioMixer Group")] private AudioMixer mixer;
    [SerializeField] [Tooltip("Corrseponding exposed AudioMixer volume parameter name")] private string mixerVarName;

    private Bus audioBus;
    [SerializeField] private string audioBusRef;

    [SerializeField] private GameObject muteIcon, audioIcon, audioLowIcon, audioMidIcon, audioHighIcon;

    [SerializeField] private Button[] arrowButtons;

    /// <summary>
    /// 
    /// </summary>
    private bool muted = false;

    /// <summary>
    /// 
    /// </summary>
    private float prevVal = 10;

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        Invoke("CheckSliderVal", 0.05f);
        audioBus = RuntimeManager.GetBus(audioBusRef);
    }

    /// <summary>
    /// 
    /// </summary>
    private void CheckSliderVal()
    {
        if (slider.value == 0)
        {
            muted = true;
            audioIcon.SetActive(false);
            muteIcon.SetActive(true);
        }

        else
        {
            muted = false;
            prevVal = slider.value;
            audioIcon.SetActive(true);
            muteIcon.SetActive(false);
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
        switch (value)
        {
            case (0):
                arrowButtons[0].GetComponent<Image>().enabled = false;
                arrowButtons[0].GetComponent<Button>().enabled = false;
                arrowButtons[1].GetComponent<Image>().enabled = true;
                arrowButtons[1].GetComponent<Button>().enabled = true;
                break;
            case (20):
                arrowButtons[0].GetComponent<Image>().enabled = true;
                arrowButtons[0].GetComponent<Button>().enabled = true;
                arrowButtons[1].GetComponent<Image>().enabled = false;
                arrowButtons[1].GetComponent<Button>().enabled = false;
                break;
            default:
                arrowButtons[0].GetComponent<Image>().enabled = true;
                arrowButtons[0].GetComponent<Button>().enabled = true;
                arrowButtons[1].GetComponent<Image>().enabled = true;
                arrowButtons[1].GetComponent<Button>().enabled = true;
                break;
        }

        float newVal;

        if (value == 0)
        {
            if (!muted)
                ToggleMute();

            newVal = 0.001f;
        }
        else
        {
            if(value < 7)
            {
                audioLowIcon.SetActive(true);
                audioMidIcon.SetActive(false);
                audioHighIcon.SetActive(false);
            }
            else if(value < 15)
            {
                audioLowIcon.SetActive(true);
                audioMidIcon.SetActive(true);
                audioHighIcon.SetActive(false);
            }
            else
            {
                audioLowIcon.SetActive(true);
                audioMidIcon.SetActive(true);
                audioHighIcon.SetActive(true);
            }

            prevVal = value;
            newVal = value / 20;

            if (muted)
                ToggleMute();
        }

        //mixer.SetFloat(mixerVarName, Mathf.Log10(newVal) * 20);

        audioBus.setVolume(newVal);

        PlayerPrefs.SetFloat(mixerVarName, value);

        volDisplay.text = (value * 5) + "%";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void SetBusVolume(float value)
    {
        audioBus = RuntimeManager.GetBus(audioBusRef);
        audioBus.setVolume(value);
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
                audioIcon.SetActive(false);
                muteIcon.SetActive(true);
                break;

            case (false):
                SetLevel(prevVal);
                slider.value = prevVal;
                audioIcon.SetActive(true);
                muteIcon.SetActive(false);
                break;
        }
    }
}
