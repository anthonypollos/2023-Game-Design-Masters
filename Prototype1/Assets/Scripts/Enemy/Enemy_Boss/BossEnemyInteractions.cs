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
        Break();
    }

    public override void Break()
    {
        base.Break();
    }

    public override void Death()
    {
        base.Death();
    }

    public override void Pulled(IsoAttackManager player = null)
    {
        Break();
    }
}
