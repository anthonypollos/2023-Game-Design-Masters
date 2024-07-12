using System.Collections;
using UnityEngine;

/// <summary>
/// Purpose: Disable realtime lights that are far enough away from the player that they shouldn't be on.
/// A system like this will be NECESSARY if we switch to realtime lights, which we need to do anyway.
/// Modified from a script by Unity discussion thread user "raycosantana"
/// https://discussions.unity.com/t/how-to-disable-point-lights-not-in-the-camera-frustum/99346
/// </summary>

public class LightDistanceDisabler : MonoBehaviour
{
    [Tooltip("This is the maximum distance that the light can be active")]
    public float availableDistance = 50;
    private float Distance;
    private Light Lightcomponent;
    private GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        //Get this object's light component
        Lightcomponent = gameObject.GetComponent<Light>();
        //Find the player by searching for them with the tag
        Player = GameObject.FindGameObjectWithTag("Player");
        //start slowly checking once a second for lights outside of max distance
        InvokeRepeating(nameof(SlowUpdate), Random.Range(0, 1f), 1f);
    }


    void SlowUpdate()
    {
        //NOTE: This code is VERY inefficient.
        //Ideally, we want to enable or disable the lights only once, instead of checking constantly
        //We should replace this with a capsule raycast from the player at some point

        Distance = Vector3.Distance(Player.transform.position, transform.position);

        if (Distance < availableDistance)
        {
            Lightcomponent.enabled = true;
            if (Lightcomponent.GetComponent<LightFlicker>()) Lightcomponent.GetComponent<LightFlicker>().StartFlicker();
            return; //Return just to save a little on perf
        }
            
        if (Distance > availableDistance)
        {
            Lightcomponent.enabled = false;
            if (Lightcomponent.GetComponent<LightFlicker>()) Lightcomponent.GetComponent<LightFlicker>().StopFlicker();
            return;
        }
    }
}
