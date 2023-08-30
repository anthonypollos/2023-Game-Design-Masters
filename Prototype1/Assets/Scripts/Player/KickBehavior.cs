using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickBehavior : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("touch");
        if(other.gameObject.CompareTag("Enemy"))
        {
            //Debug.Log("hit");
            GetComponentInParent<PlayerAttackManager>().activateKick(other.gameObject);
            //Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }
}
