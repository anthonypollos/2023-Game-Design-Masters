using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    //[SerializeField] [Tooltip("")] private Slider audio

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mod"></param>
    public void AdjustVolume(int mod)
    {
        int vol = 0; //(int)slider.value;

        Debug.Log(vol);

        vol += mod;

        Debug.Log(vol);
    }
}
