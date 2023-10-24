using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyInteractions : EnemyInteractionBehaviorTemplate
{
    [SerializeField]
    [Tooltip("Damage dealt and taken when colliding with someone then launched")]
    int clashDamage = 20;
    bool launched;
    [SerializeField]
    [Tooltip("Stun time when taking damage")]
    float stunTime = 0.5f;
     void Start()
    {
        lassoed = false;
        stunned = false;
        launched = false;
        hasCollided = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (launched && !brain.moveable.isLaunched)
        {
            hasCollided = true;
            UnStunned();
            launched = false;
        }
    }
    public override void Kicked()
    {
        launched = true;
        Stunned();
        hasCollided = false;
        
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
        launched = true;
        lassoed = false;
        hasCollided = false;
        brain.an.SetBool("Lassoed", false);
    }

    public override void Break()
    {
        base.Break();
        lassoed = false;
        if (!brain.moveable.isLaunched)
        { 
        brain.an.SetBool("Lassoed", false);
        UnStunned();
        }
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
        if(!lassoed)
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
        if (!brain.moveable.isLaunched)
            UnStunned();

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (brain.moveable.isLaunched && !collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Lasso") && !hasCollided)
        {
            hasCollided = true;
            GameObject hit = collision.gameObject;
            brain.health.TakeDamage(clashDamage);
            IDamageable temp = hit.GetComponent<IDamageable>();
            if (temp != null)
            {
                temp.TakeDamage(clashDamage);
            }
            
            ITrap temp2 = hit.GetComponent<ITrap>();
            if (temp2 != null)
            {
                temp2.ActivateTrap(gameObject);
            }
            
        }
    }


}
