using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineToggle : MonoBehaviour
{
    [SerializeField] private List<Outline> outlines;

    // Start is called before the first frame update
    void Start()
    {
        //fill list with outline objects
        outlines = new List<Outline>(FindObjectsOfType<Outline>());
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

    //Add a new object to the outline list
    public void AddOutline(GameObject NewOutline)
    {
        //Debug.Log("Attempting to add " + NewOutline + " to the outline list.");
        outlines.Add(NewOutline.GetComponent<Outline>());
    }

    public void RemoveOutline(GameObject RemoveMe)
    {
        outlines.Remove(RemoveMe.GetComponent<Outline>());
    }
}
