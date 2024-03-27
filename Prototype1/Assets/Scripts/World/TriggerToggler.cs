using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Toggles gameobjects' active state in a list when something enters the trigger bounds
/// Author: Sean Lee 2/20/24
/// </summary>

public class TriggerToggler : MonoBehaviour
{
    [Header("WARNING:\nGenericItems will delete themselves upon being disabled!\nDO NOT allow GenericItems to disable!")]
    [Tooltip("All the objects enabled/disabled by this Lever.\nNOTE: These are NOT IToggleables, these are GameObjects")]
    [SerializeField] private List<GameObject> toggleObjects;
    [Space(10)]
    [Header("Activator Options")]
    [Tooltip("Do we want to limit the objects that activate this trigger to specific gameobjects from a list?\nIf False, uses accepted Tags instead.")]
    [SerializeField] private bool useAcceptedList;
    [Tooltip("The accepted objects that can activate this trigger.\nRequires useAcceotedList to be enabled")]
    [SerializeField] private List<GameObject> acceptedObjects;
    [Tooltip("The accepted gameobject tags that can activate this trigger")]
    [SerializeField] private List<string> acceptedTags;
    [Space(10)]
    [Header("Usability Options")]
    [Tooltip("Does this trigger only happen once?")]
    [SerializeField] private bool onlyOnce;
    [Tooltip("Does this trigger activate on exit as well?\nDoes not work with onlyOnce.")]
    [SerializeField] private bool triggerOnExit;

    private OutlineToggle outlineManager;

    //We use this to make sure that the trigger has been run once already.
    private bool hasRun = false;
    //We use this to make sure the toggle only happens when something enters the trigger for the first time.
    //This should prevent the toggle from happening again if two accepted objects are inside
    private bool inTrigger = false;

    private void Start()
    {
        outlineManager = FindObjectOfType<OutlineToggle>();
    }


    private void OnTriggerEnter(Collider other)
    {
        //First off, make sure we're an accepted object and the trigger is empty
        if (CheckAccepted(other.gameObject) && (!inTrigger))
        {
            inTrigger = true;
            foreach (GameObject obj in toggleObjects)
            {
                Toggle(obj);
            }
            hasRun = true;
        }
        //If we're set to only run once and the run was valid, destroy ourselves
        if (onlyOnce && hasRun) Destroy(this);
    }

    private void OnTriggerExit(Collider other)
    {
        //Check to make sure the object exiting our trigger is accepted and inTrigger is true
        if (CheckAccepted(other.gameObject) && inTrigger)
        {
            //Check if we retrigger on exit
            if (triggerOnExit)
            {
                foreach (GameObject obj in toggleObjects)
                {
                    Toggle(obj);
                }
            }
            //After we've run our code, disable inTrigger so we know the trigger is empty
            inTrigger = false;
        }
    }

    //check to see if this object is acceptable
    private bool CheckAccepted(GameObject obj)
    {
        //If the object is on the accepted list, return true
        if (useAcceptedList && acceptedObjects.Contains(obj)) return true;
        //If we're going by tags and the object's tag is on the tag list, return true
        if ((!useAcceptedList) && acceptedTags.Contains(obj.gameObject.tag)) return true;
        //If neither of those checks run, return false
        return false;
    }

    //Toggle the gameobject active state
    private void Toggle(GameObject obj)
    {
        if (obj.activeSelf == false)
        {
            //first, set active
            obj.SetActive(true);
            //If the object has an outline, add it to the manager
            foreach (Transform child in obj.transform)
            {
                //Check the children of the respawned object. Find whatever child has the outline and add it
                if (child.GetComponent<Outline>() != null)
                {
                    outlineManager.AddOutline(child.gameObject);
                    //de-activate the outline
                    child.GetComponent<Outline>().enabled = false;
                }
            }
        }  
        else if (obj.activeSelf == true) obj.SetActive(false);
    }
}
