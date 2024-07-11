using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MeleeEnemyInteractions : EnemyInteractionBehaviorTemplate
{

    [SerializeField] GameObject KickedParticle;

    //[SerializeField] private JukeBox jukebox;

    [SerializeField] MeleeAttackBehavior mab;

    [SerializeField] private GameObject stunParticle;

    private void Awake()
    {
        //jukebox.SetTransform(transform);
    }
    void Start()
    {
        lassoed = false;
        stunned = false;
        launched = false;
        hasCollided = true;
        //mab = GetComponentInChildren<MeleeAttackBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
       if (!brain.moveable.isLaunched && !coroutineRunning)
        {
            if (launched)
            {
                hasCollided = true;
                UnStunned();
                launched = false;
            }
            else if (stunned)
            {
                UnStunned();
            }
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

    public override void Pulled(IsoAttackManager player = null)
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
        //if we're stunned by a way other than being lassoed (such as spikes) enable the stun particle
        if (!lassoed && stunParticle != null) stunParticle.SetActive(true);
        //brain.an.SetBool("Stunned", true);
        brain.an.SetBool("Attacking", false);
        brain.an.SetBool("Tendriled", true);
    }

    protected override void UnStunned()
    {
        if(!lassoed && !launched && !brain.moveable.isLaunched)
        {
            if (stunParticle != null) stunParticle.SetActive(false);
            brain.an.SetBool("Tendriled", false);
            brain.an.SetTrigger("NextState");
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

    public void IgnoreInteractables()
    {
        if (mab != null)
        {
            mab.IgnoreAllInteractables();
        }
        else
        {
            Debug.Log("MAB == null");
        }
    }

    public void RecognizeInteractables()
    {
        if (mab != null)
        {
            mab.RecognizeAllInteractables();
        }
        else
        {
            Debug.Log("MAB == null");
        }
    }
}
