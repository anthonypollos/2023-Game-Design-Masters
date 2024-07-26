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

    [SerializeField] private BossEnemyAttacks bossAttacks;
    [SerializeField] private BossEnemyInteractions bossInteractions;
    [SerializeField] private BossEnemyHealth bossHeath;
    [SerializeField] private Animator bossHUDAnim;

    public void BossWindUp(int Int)
    {
        bossAttacks.WindUpTrigger(Int);
    }

    public void BossAttackEnd()
    {
        bossAttacks.AttackEnd();
    }

    public void BossDashing(int Int)
    {
        bossAttacks.Dashing(Int);
    }

    public void BossDashCollide()
    {
        bossInteractions.DashCollide();
    }

    public void BossTriggerAttack(int Int)
    {
        bossAttacks.TriggerAttack(Int);
    }

    public void BossWindDownTrigger(int Int)
    {
        bossAttacks.WindDownTrigger(Int);
    }

    public void BossDeath()
    {
        bossHeath.Death();
    }

    public void TriggerBossHud(string trigger)
    {
        bossHUDAnim.SetTrigger(trigger);
    }
}
