using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyInteractionBehaviorTemplate : MonoBehaviour, IPullable, IKickable
{
    [HideInInspector]
    protected bool lassoed;
    [HideInInspector]
    protected bool hasCollided;
    [HideInInspector]
    public bool stunned;
    [HideInInspector]
    public EnemyBrain brain;
    
    

    public abstract void Kicked();
    public abstract void Lassoed();
    public abstract void Pulled();
    public abstract void Stagger();
    protected abstract void Stunned();
    protected abstract void UnStunned();

}
