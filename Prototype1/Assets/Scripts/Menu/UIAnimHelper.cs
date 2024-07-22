using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// An assortment of various functions to supplement UI anims, since anim functions
/// and animatons are limited in many ways, especially in relation to text
/// </summary>
public class UIAnimHelper : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private string[] headers;

    [SerializeField] private TextMeshProUGUI addHeaderText;
    [SerializeField] private string addHeader;

    [SerializeField] private ToggleObjs[] toggles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHeader(int index)
    {
        headerText.text = headers[index];
    }

    public void SetAddHeader()
    {
        addHeaderText.text = addHeader;
    }

    public void ToggleObjects()
    {
        foreach (ToggleObjs obj in toggles)
        {
            obj.obj.SetActive(obj.toggle);
        }
    }
}

[System.Serializable]
public class ToggleObjs
{
    public GameObject obj;
    public bool toggle;
}