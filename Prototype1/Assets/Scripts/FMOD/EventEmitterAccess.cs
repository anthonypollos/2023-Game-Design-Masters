using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class EventEmitterAccess : MonoBehaviour
{
    public GameObject AManager;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            AManager.GetComponent<FMODUnity.StudioEventEmitter>().Stop();
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //AManager.GetComponent<FMODUnity.StudioEventEmitter>().Play();
        }
    }
}
