using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class NewRangedEnemyInteractions : EnemyInteractionBehaviorTemplate
{

    [SerializeField]
    [Tooltip("Damage dealt and taken when colliding with someone then launched")]
    int clashDamage = 20;

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

    public override void Pulled()
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
        brain.an.SetBool("Stunned", true);
        brain.an.SetBool("Attacking", false);
    }

    protected override void UnStunned()
    {
        if (!lassoed && !launched && !brain.moveable.isLaunched)
        {
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
                jukebox.PlaySound(1);
            }

            ITrap temp2 = hit.GetComponent<ITrap>();
            if (temp2 != null)
            {
                temp2.ActivateTrap(gameObject);
            }

        }
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
