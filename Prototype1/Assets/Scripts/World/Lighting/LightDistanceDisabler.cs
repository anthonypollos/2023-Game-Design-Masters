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
    }

    // We use FixedUpdate instead of Update because we don't want *every* light in *every* map to run this *every* frame
    void FixedUpdate()
    {
        //NOTE: This code is VERY inefficient.
        //Ideally, we want to enable or disable the lights only once, instead of checking constantly
        //We should replace this with a capsule raycast from the player at some point

        Distance = Vector3.Distance(Player.transform.position, transform.position);

        if (Distance < availableDistance) Lightcomponent.enabled = true;
        if (Distance > availableDistance) Lightcomponent.enabled = false;
    }
}
