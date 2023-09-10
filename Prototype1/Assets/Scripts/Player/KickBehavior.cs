using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickBehavior : MonoBehaviour
{
    [SerializeField] int dmg = 5;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("touch");
        if(other.gameObject.GetComponentInParent<IKickable>()!=null)
        {
            //Debug.Log("hit");
            GetComponentInParent<ICanKick>().ActivateKick(other.gameObject);
            //Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
        if(other.gameObject.GetComponentInParent<IDamageable>()!=null)
        {
            //Debug.Log("deal damage");
            other.gameObject.GetComponentInParent<IDamageable>().TakeDamage(dmg);
        }
    }
}
