using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpikes : MonoBehaviour, ITrap
{
    [SerializeField] int dmg = 20;


    public void ActivateTrap(GameObject target)
    {
        IDamageable temp = target.GetComponent<IDamageable>();
        if(temp!= null)
        {
            temp.TakeDamage(dmg);
        }
    }
}
