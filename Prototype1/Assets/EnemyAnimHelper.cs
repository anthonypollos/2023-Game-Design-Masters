using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimHelper : MonoBehaviour
{
    [SerializeField] private RangedEnemyAttacks rangedAttacks;
    [SerializeField] private EnemyHealth enemyHealth;

    public void Windup(int Int)
    {
        rangedAttacks.WindUpTrigger(Int);
    }

    public void ShootProjectile()
    {
        rangedAttacks.ShootProjectile();
    }

    public void AttackEnd()
    {
        rangedAttacks.AttackEnd();
    }

    public void Death()
    {
        enemyHealth.Death();
    }
}
