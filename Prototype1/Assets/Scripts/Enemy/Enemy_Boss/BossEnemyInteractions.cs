using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyInteractions : EnemyInteractionBehaviorTemplate
{
    [HideInInspector]
    public BossEnemyBrain bossBrain;
    public override void Kicked()
    {
        return;
    }

    public override void Lassoed()
    {
        brain.an.ResetTrigger("Damaged");
        brain.an.SetTrigger("Damaged");
        Break();
    }

    public override void Break()
    {
        base.Break();
    }

    public override void Death()
    {
        brain.an.SetTrigger("Death");
    }

    public override void Pulled(IsoAttackManager player = null)
    {
        Break();
    }

    public void DashCollide()
    {
        return;
    }
}
