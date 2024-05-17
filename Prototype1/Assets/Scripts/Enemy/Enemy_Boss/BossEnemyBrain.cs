using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyBrain : EnemyBrain
{
    [HideInInspector]
    public BossEnemyAttacks bossAttacks;
    [HideInInspector]
    public BossEnemyHealth bossHealth;
    [HideInInspector]
    public BossEnemyInteractions bossInteractions;
    [HideInInspector]
    public BossEnemyMovevment bossMovevment;

    [SerializeField]
    bool debugToggleTrigger = false;
    protected override void Starting()
    {
        base.Starting();
        bossAttacks = attack.GetComponent<BossEnemyAttacks>();
        bossAttacks.bossBrain = this;
        bossHealth = health.GetComponent<BossEnemyHealth>();
        bossHealth.bossBrain = this;
        bossInteractions = interaction.GetComponent<BossEnemyInteractions>();
        bossInteractions.bossBrain = this;
        bossMovevment = movement.GetComponent<BossEnemyMovevment>();
        bossMovevment.bossBrain = this;
    }

    private void FixedUpdate()
    {
        if(!interaction.stunned && state == EnemyStates.CHARGING)
        {
            CheckMovement();
        }
    }
    protected override void Updating()
    {
        if (state != EnemyStates.DEAD)
        {
            //if not stunned and not attacking
            if (!interaction.stunned && state == EnemyStates.NOTHING)
            {
                CheckMovement();
                CheckRotation();
                CheckAttack();
                if(isAggro)
                    CheckArea();
            }
            else if(!interaction.stunned && state == EnemyStates.ENRAGED)
            {
                CheckAttack();
            }
            //if stunned stop all movement calculations
            else if (interaction.stunned && moveable != null)
            {
                if (!moveable.isLaunched)

                    movement.Stop();
            }
        }

        if(debugToggleTrigger)
        {
            debugToggleTrigger = false;
            if(state == EnemyStates.ENRAGED)
            {
                Calm();
            }
        }
    }

    protected override void CheckAttack()
    {
        base.CheckAttack();
    }

    protected override void CheckMovement()
    {
        base.CheckMovement();
    }

    public void Enrage()
    {
        state = EnemyStates.ENRAGED;
    }

    public void Calm()
    {
        state = EnemyStates.NOTHING;
    }
}
