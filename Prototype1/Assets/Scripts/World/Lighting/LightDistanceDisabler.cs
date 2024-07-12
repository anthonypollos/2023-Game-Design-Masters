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

    private bool isOn;


    // Start is called before the first frame update
    void Start()
    {
        //Get this object's light component
        Lightcomponent = gameObject.GetComponent<Light>();
        //Find the player by searching for them with the tag
        Player = GameObject.FindGameObjectWithTag("Player");

        //If this light does not have a collider on it, begin the old repeating method
        if (GetComponent<Collider>() == null)
        {
            InvokeRepeating(nameof(SlowUpdate), Random.Range(0, 1f), 1f);
        }
        TurnOff();
    }

    public void TurnOn()
    {
        Lightcomponent.enabled = true;
        if (Lightcomponent.GetComponent<LightFlicker>()) Lightcomponent.GetComponent<LightFlicker>().StartFlicker();
        isOn = true;
    }

    public void TurnOff()
    {
        Lightcomponent.enabled = false;
        if (Lightcomponent.GetComponent<LightFlicker>()) Lightcomponent.GetComponent<LightFlicker>().StopFlicker();
        isOn = false;
    }

    void SlowUpdate()
    {

        Distance = Vector3.Distance(Player.transform.position, transform.position);

        if (Distance <= availableDistance && !isOn)
        {
            TurnOn();
            return; //Return just to save a little on perf
        }
            
        if (Distance > availableDistance && isOn)
        {
            TurnOff();
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Player)
        {
            TurnOn();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Player)
        {
            TurnOff();
        }
    }
}
