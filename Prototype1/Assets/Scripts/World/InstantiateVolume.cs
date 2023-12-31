using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *  Purpose: A Volume that instantiates an object at a chosen point.
 *  Created for the asset "Art Gallery" for particles.
 *  Assumes that the trigger activator is the player at the moment, but this
 *  can be changed with a serialized tag field later.
 *  Sean Lee 10/18/23
 */
public class InstantiateVolume : MonoBehaviour
{
    [Tooltip("The object to instantiate.")]
    [SerializeField] GameObject InstantiatedObject;
    [Tooltip("The amount of time that the object lives for. 0 makes it live forever.")]
    [SerializeField] private float InstantiatedObjectLifetime = 0f;
    [Tooltip("The place where the object spawns.")]
    [SerializeField] Vector3 SpawnLocation;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //not bothering with the rotation. This can be changed to a serialized value if we REALLY want it.
            GameObject Insantiated = Instantiate(InstantiatedObject, SpawnLocation, Quaternion.Euler(transform.forward));
            if (InstantiatedObjectLifetime != 0f) Destroy(Insantiated, InstantiatedObjectLifetime);
        }
    }
}
