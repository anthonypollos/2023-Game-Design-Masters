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
            centerTownSquareObject.CystDestroyed();
            base.Die();
        }
    }

}
