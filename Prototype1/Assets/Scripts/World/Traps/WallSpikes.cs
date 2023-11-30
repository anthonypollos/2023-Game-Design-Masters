using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpikes : MonoBehaviour, ITrap
{
    [SerializeField] int dmg = 20;
    [SerializeField] float bleedTime = 3f;

    public void ActivateTrap(GameObject target)
    {
        IDamageable temp = target.GetComponent<IDamageable>();
        if(temp!= null)
        {
            temp.TakeDamage(dmg);
        }
        Bleedable bleedable = target.GetComponent<Bleedable>();
        if(bleedable!=null)
        {
            bleedable.Activate(bleedTime);
        }
    }
}
