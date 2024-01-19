using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MeleeEnemyInteractions : EnemyInteractionBehaviorTemplate
{

    [SerializeField] GameObject KickedParticle;

    [SerializeField] private JukeBox jukebox;

    private void Awake()
    {
        jukebox.SetTransform(transform);
    }
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
            launched = false;
            UnStunned();
        }
    }
    public override void Kicked()
    {
        if (brain.state != EnemyStates.ATTACKING)
        {
            launched = true;
            Stunned();
            hasCollided = false;

            //If there is a kicked particle, create it.
            if (KickedParticle != null)
            {
                //create the particle
                GameObject vfxobj = Instantiate(KickedParticle, gameObject.transform.position, Quaternion.identity);
                //destroy the particle
                Destroy(vfxobj, 4);
            }
        }

    }

    public override void Lassoed()
    {
        StopAllCoroutines();
        lassoed = true;
        base.Lassoed();
        Stunned();
        brain.an.SetBool("Tendriled", true);
    }

    public override void Pulled()
    {
        base.Pulled();
        launched = true;
        //jukebox.PlaySound(0);
        //lassoed = false;
        hasCollided = false;
        //brain.an.SetBool("Lassoed", false);
    }

    public override void Break()
    {
        base.Break();
        lassoed = false;
        if (!brain.moveable.isLaunched)
        { 
            brain.an.SetBool("Tendriled", false);
            brain.an.SetTrigger("TendrilBreak");
            UnStunned();
        }
        else
        {
            brain.an.SetTrigger("NextState");
        }
    }

    public override void Stagger()
    {
        base.Stagger();
    }

    protected override void Stunned()
    {
        base.Stunned();
        stunned = true;
        //brain.an.SetBool("Stunned", true);
        brain.an.SetBool("Attacking", false);
    }

    protected override void UnStunned()
    {
        if(!lassoed && !launched && !brain.moveable.isLaunched)
        {
            brain.an.SetBool("Tendriled", false);
            //brain.an.SetBool("Stunned", false);
            base.UnStunned();
        }
    }

    protected override IEnumerator Staggered()
    {
        brain.an.SetTrigger("Damaged");
        StopCoroutine(base.Staggered());
        StartCoroutine(base.Staggered());
        yield break;

    }

    public override void Death()
    {
        brain.an.SetTrigger("Death");
    }


    public override void Stun(float time)
    {
        base.Stun(time);
    }

    protected override IEnumerator StunTimer(float seconds)
    {
        StopCoroutine(base.StunTimer(seconds));
        StartCoroutine(base.StunTimer(seconds));
        yield break;
    }
}
