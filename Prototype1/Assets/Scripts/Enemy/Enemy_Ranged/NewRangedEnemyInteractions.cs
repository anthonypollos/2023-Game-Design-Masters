using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class NewRangedEnemyInteractions : EnemyInteractionBehaviorTemplate
{


    [SerializeField] GameObject KickedParticle;
    [SerializeField] private JukeBox jukebox;
    [SerializeField] private GameObject stunParticle;

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
        UnStunned();

    }

    public override void Lassoed()
    {
        StopAllCoroutines();
        lassoed = true;
        base.Lassoed();
        Stunned();
        brain.an.SetBool("Lassoed", true);
    }

    public override void Pulled(IsoAttackManager player = null)
    {
        base.Pulled();
        launched = true;
        hasCollided = false;
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
        base.Stagger();
    }

    protected override void Stunned()
    {
        base.Stunned();
        stunned = true;
        //if we're stunned by a way other than being lassoed (such as spikes) enable the stun particle
        if (!lassoed && stunParticle != null) stunParticle.SetActive(true);
        brain.an.SetBool("Stunned", true);
        brain.an.SetBool("Attacking", false);
    }

    protected override void UnStunned()
    {
        if (!lassoed && !launched && !brain.moveable.isLaunched)
        {
            if (stunParticle != null) stunParticle.SetActive(false);
            brain.an.SetBool("Stunned", false);
            base.UnStunned();
        }
    }

    protected override IEnumerator Staggered()
    {
        StopCoroutine(base.Staggered());
        StartCoroutine(base.Staggered());
        yield break;

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
