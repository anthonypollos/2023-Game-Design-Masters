using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownSquareCysts : EnemyHealth
{
    [SerializeField] TownSquareCenter centerTownSquareObject;
    protected override void Die()
    {
        if (!dead)
        {
            if (centerTownSquareObject != null)
            {
                centerTownSquareObject.CystDestroyed();
            }
            base.Die();
        }
    }

}
