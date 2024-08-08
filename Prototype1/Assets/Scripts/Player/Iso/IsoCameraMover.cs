using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoCameraMover : MonoBehaviour
{

    private GameObject mainCamera;
    [Tooltip("The amount of time the camera will hold on this object after it's spawned.")][SerializeField] private float holdTime = 1f;
    [Tooltip("The smoothing (movement) rate of the camera as it moves to the target.")] [SerializeField] private float cameraMoveRate = 5f;
    [Tooltip("ENEMY ONLY!\nMove the camera on enemy aggro.")] [SerializeField] private bool cameraOnAggro = false;

    private float cameraSmoothing;

    // Start is called before the first frame update
    void Start()
    {
        //1. Get the main camera
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        //2. Check to make sure that the camera's current target is the player
        if (mainCamera.GetComponent<IsoCamera>().isTargetPlayer() && !cameraOnAggro)
        {
            StealCamera();
        }
    }

    private IEnumerator GoBackToPlayer(float time)
    {
        yield return new WaitForSeconds(time);
        //Set the camera's smoothing rate back to its original
        mainCamera.GetComponent<IsoCamera>()._smoothing = cameraSmoothing;

        //If this check is true, it means that the camera smoothing rate *after* resetting it is still the same as the slowed rate from here.
        //This should NOT happen and means that cameraSmoothing was somehow set to be equal to cameraMoveRate at some point.
        //If this happens, we assume that the correct number is 10f (our level default) and set it to this.
        if (mainCamera.GetComponent<IsoCamera>()._smoothing == cameraMoveRate)
        {
            cameraSmoothing = 10f; //superfluous but better safe than sorry.
            mainCamera.GetComponent<IsoCamera>()._smoothing = 10f;
        }
        mainCamera.GetComponent<IsoCamera>().RevertTarget();
    }

    public void StealCamera()
    {
        //Set the private float here to whatever the default smoothing value is on the camera
        cameraSmoothing = mainCamera.GetComponent<IsoCamera>()._smoothing;
        //set the camera's smoothing to our set value.
        mainCamera.GetComponent<IsoCamera>()._smoothing = cameraMoveRate;
        //If the target is the player, set the target to this
        mainCamera.GetComponent<IsoCamera>().ChangeTarget(gameObject);
        //After we've set the camera target to this, wait for holdtime and then revert to the player
        StartCoroutine(GoBackToPlayer(holdTime));
    }

    public bool GetCamOnAggro()
    {
        return cameraOnAggro;
    }
}
