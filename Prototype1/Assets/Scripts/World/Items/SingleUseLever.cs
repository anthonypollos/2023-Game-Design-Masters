using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A single-use pullable object that disables itself after being pulled.
/// This variant toggles gameobject active states instead of IToggleables
/// Author: Sean Lee 2/20/24
/// </summary>

public class SingleUseLever : MonoBehaviour, IPullable
{
    [Tooltip("All the objects enabled/disabled by this Lever.\nNOTE: These are NOT IToggleables, these are GameObjects")]
    public List<GameObject> toggleObjects;

    public void Lassoed()
    {
        return;
    }

    public void Pulled()
    {
        foreach(GameObject obj in toggleObjects)
        {
            Toggle(obj);
        }
        //Disable the collider so we can't grab this again.
        GetComponent<Collider>().enabled = false;
    }

    public void Break()
    {

    }

    private void Toggle(GameObject obj)
    {
        if (obj.activeSelf == false) obj.SetActive(true);
        else if (obj.activeSelf == true) obj.SetActive(false);
    }

}
