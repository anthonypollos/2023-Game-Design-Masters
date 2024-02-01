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
    [Tooltip("The name of the camera's parent that follows the player\nDefault is 'CameraTarget'")]
    [SerializeField] string cameraName = "CameraTarget";
    
    private GameObject playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        //If we don't have useTags on, set the tag to Player
        if (!useTags) acceptedTag = "Player";

        //If we use useTeleObject, set the teleport destination to the object's position.
        if (useTeleObject) teleportDestination = teleportDestObject.transform.position;

        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void OnTriggerEnter(Collider other)
    {
        //If the tag is accepted, teleport the object that activated this trigger
        if (other.gameObject.tag == acceptedTag) Teleport(other.gameObject);
    }

    void Teleport(GameObject Teleportee)
    {
        //Set the teleportee's position to the destination
        Teleportee.transform.position = teleportDestination;

        //TEMP HACK
        //Teleport the camera, maintaining the offset from the player
        if (teleportCamera) playerCamera.transform.position = new Vector3(teleportDestination.x, teleportDestination.y + 26.83f, teleportDestination.z - 18.37262f);

        //if the object that teleported has moveable (which it always should but an extra check won't hurt)
        //force any dashing or anything to stop
        if (Teleportee.GetComponent<Moveable>() != null) Teleportee.GetComponent<Moveable>().ForceStop();


    }
}
