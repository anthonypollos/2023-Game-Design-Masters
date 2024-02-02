using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose: A trigger that teleports the player, or, potentially, other objects
/// </summary>

public class TriggerTeleport : MonoBehaviour
{
    [Tooltip("Should we use tags?\nIf false, assume we use the player")]
    [SerializeField] bool useTags = false;
    [Tooltip("The tag accepted for teleporting.\nRequires useTags")]
    [SerializeField] string acceptedTag;
    
    [Tooltip("The destination to be teleported to.")]
    [SerializeField] Vector3 teleportDestination;

    [Tooltip("Should we teleport to an object instead of a set destination?")]
    [SerializeField] bool useTeleObject = false;
    [Tooltip("Should the destination be dynamic?\nRequires useTeleObject")]
    [SerializeField] bool dynamicDestination = false;
    [Tooltip("The object to use as a teleport destination.\nRequires useTeleObject")]
    [SerializeField] GameObject teleportDestObject;
    
    [Tooltip("Should the camera come with?")]
    [SerializeField] bool teleportCamera = true;
    //[Tooltip("The name of the camera's parent that follows the player\nDefault is 'CameraTarget'")]
    //[SerializeField] string cameraName = "CameraTarget";

    [Tooltip("Should the object teleport relative to where it entered the trigger instead of just to the center?")]
    [SerializeField] bool relativeTeleport = false;

    private GameObject playerCamera;
    private GameObject player;
    private Vector3 cameraDiff;
    private Vector3 relativeOffset;
    private Vector3 tempDest;

    // Start is called before the first frame update
    void Start()
    {
        //If we don't have useTags on, set the tag to Player
        if (!useTags) acceptedTag = "Player";

        //If we use useTeleObject, set the teleport destination to the object's position.
        if (useTeleObject) teleportDestination = teleportDestObject.transform.position;

        
        //If TeleportCamera is on, assume we're teleporting the player
        if (teleportCamera)
        {
            //Set the camera
            playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
            //Set the player
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //if we want to teleport with an offset, get the offset
        if (relativeTeleport)
        {
            relativeOffset = new Vector3(transform.position.x - other.transform.position.x, transform.position.y - other.transform.position.y, transform.position.z - other.transform.position.z);
        }

        //If the destination is dynamic, update it just before teleporting
        //This is significantly cheaper than using Update or FixedUpdate
        if (dynamicDestination) teleportDestination = teleportDestObject.transform.position;

        //Find the distance between the player and the camera if we're teleporting the camera as well
        //Set in OnTriggerEnter because, if we eventually make the camera dynamic and able to zoom in and out, we'll need to get this as late as possible before teleporting
        if (teleportCamera) cameraDiff = new Vector3((player.transform.position.x - playerCamera.transform.position.x), (player.transform.position.y - playerCamera.transform.position.y), (player.transform.position.z - playerCamera.transform.position.z));

        //If the tag is accepted, teleport the object that activated this trigger
        if (other.gameObject.tag == acceptedTag) Teleport(other.gameObject);
    }

    void Teleport(GameObject Teleportee)
    {
        //if the teleporting is relative instead of just to the origin of the destination, we need to account for where the player entered the trigger
        if (relativeTeleport)
        {
            //Grab the original destination and save it
            tempDest = teleportDestination;

            //subtract the offset from the destination
            teleportDestination = new Vector3(teleportDestination.x - relativeOffset.x, teleportDestination.y - relativeOffset.y, teleportDestination.z - relativeOffset.z);
        }
        //Set the teleportee's position to the destination
        Teleportee.transform.position = teleportDestination;


        //if we teleport the camera, set the camera's position to the destination minus the difference between it and the player
        if (teleportCamera) playerCamera.transform.position = new Vector3(teleportDestination.x, teleportDestination.y - cameraDiff.y, teleportDestination.z - cameraDiff.z);

        //if the object that teleported has moveable (which it always should but an extra check won't hurt) force any dashing to stop
        if (Teleportee.GetComponent<Moveable>() != null) Teleportee.GetComponent<Moveable>().ForceStop();

        //FINALLY (put this at the end) if we're doing a relative teleport, reset teleportDestination after we've already teleported the object
        if (relativeTeleport) teleportDestination = tempDest;
    }
}
