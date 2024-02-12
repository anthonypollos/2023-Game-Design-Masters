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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
