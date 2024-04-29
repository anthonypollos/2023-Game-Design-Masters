using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineToggle : MonoBehaviour
{
    [SerializeField] private List<Outline> outlines;
    bool on;

    // Start is called before the first frame update
    void Start()
    {
        //fill list with outline objects
        outlines = new List<Outline>(FindObjectsOfType<Outline>());
        on = false;
        ToggleOutline(false);
    }

    public void ToggleOutline()
    {
        ToggleOutline(!on);
    }

    public void ToggleOutline(bool toggle)
    {
        on = toggle;
        foreach (Outline outline in outlines)
        {
            if (outline != null)
                outline.enabled = on;
        }
    }

    //Add a new object to the outline list
    public void AddOutline(GameObject NewOutline)
    {
        //Debug.Log("Attempting to add " + NewOutline + " to the outline list.");
        Outline outline = NewOutline.GetComponent<Outline>();
        outlines.Add(outline);
        outline.enabled = on;

    }

    public void RemoveOutline(GameObject RemoveMe)
    {
        outlines.Remove(RemoveMe.GetComponent<Outline>());
    }
}

