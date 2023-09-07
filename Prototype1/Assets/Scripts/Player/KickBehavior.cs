using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickBehavior : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("touch");
        if(other.gameObject.GetComponent<IKickable>()!=null)
        {
            //Debug.Log("hit");
            GetComponentInParent<ICanKick>().ActivateKick(other.gameObject);
            //Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }
}
