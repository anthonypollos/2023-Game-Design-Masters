using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineToggle : MonoBehaviour
{
    [SerializeField] private Outline[] outlines;

    // Start is called before the first frame update
    void Start()
    {
        outlines = FindObjectsOfType<Outline>();
        ToggleOutline(false);
    }

    public void ToggleOutline(bool toggle)
    {
        foreach(Outline outline in outlines)
        {
            if(outline != null)
                outline.enabled = toggle;
        }
    }
}
