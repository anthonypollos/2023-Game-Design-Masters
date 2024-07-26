using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoCameraMover : MonoBehaviour
{

    private GameObject mainCamera;
    [Tooltip("The amount of time the camera will hold on this object after it's spawned.")][SerializeField] private float holdTime = 1f;
    [Tooltip("ENEMY ONLY!\nMove the camera on enemy aggro.")] [SerializeField] private bool cameraOnAggro = false;


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
        mainCamera.GetComponent<IsoCamera>().RevertTarget();
    }

    public void StealCamera()
    {
        //3. If the target is the player, set the target to this
        mainCamera.GetComponent<IsoCamera>().ChangeTarget(gameObject);
        //4. After we've set the camera target to this, wait for holdtime and then revert to the player
        StartCoroutine(GoBackToPlayer(holdTime));
    }

    public bool GetCamOnAggro()
    {
        return cameraOnAggro;
    }
}
