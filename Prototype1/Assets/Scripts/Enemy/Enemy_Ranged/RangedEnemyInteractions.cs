using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyInteractions : EnemyInteractionBehaviorTemplate
{
    [SerializeField]
    [Tooltip("Stun time when taking damage")]
    float stunTime = 1.5f;
    void Start()
    {
        lassoed = false;
        stunned = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void Kicked()
    {
        UnStunned();

    }

    public override void Lassoed()
    {
        lassoed = true;
        base.Lassoed();
        Stunned();
        brain.an.SetBool("Lassoed", true);
    }

    public override void Pulled()
    {
        base.Pulled();
        brain.health.TakeDamage(9999999);
    }

    public override void Break()
    {
        base.Break();
        lassoed = false;
        brain.an.SetBool("Lassoed", false);
        UnStunned();
    }

    public override void Stagger()
    {
        StopCoroutine(Staggered());
        StartCoroutine(Staggered());
    }

    protected override void Stunned()
    {
        base.Stunned();
        stunned = true;
        brain.an.SetBool("Stunned", true);
        brain.an.SetBool("Attacking", false);
    }

    protected override void UnStunned()
    {
        if (!lassoed)
        {
            brain.an.SetBool("Stunned", false);
            stunned = false;
            brain.PackAggro();
        }
    }

    private IEnumerator Staggered()
    {
        Stunned();
        yield return new WaitForSeconds(stunTime);
        UnStunned();

    }

}
