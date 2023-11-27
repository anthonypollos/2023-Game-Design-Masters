using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BruteEnemyInteractions : EnemyInteractionBehaviorTemplate
{
    [SerializeField]
    [Tooltip("Damage dealt and taken when colliding with someone then launched")]
    int clashDamage = 20;

    [Tooltip("Stun Modifier")]
    [SerializeField] float stunMod = 1;

    [SerializeField] GameObject kickedPartical;

    [SerializeField] private JukeBox jukebox;

    List<GameObject> hasCollidedWith;


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
        hasCollidedWith = new List<GameObject>();
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
        if (brain.state != EnemyStates.ATTACKING)
        {
            launched = true;
            Stunned();
            hasCollided = false;

            //If there is a kicked particle, create it.
            if (kickedPartical != null)
            {
                //create the particle
                GameObject vfxobj = Instantiate(kickedPartical, gameObject.transform.position, Quaternion.identity);
                //destroy the particle
                Destroy(vfxobj, 4);
            }
        }

    }

    public override void Lassoed()
    {
        return;
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
        if (!lassoed && !brain.moveable.isLaunched)
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
        base.Stun(time * stunMod);
    }

    protected override IEnumerator StunTimer(float seconds)
    {
        StopCoroutine(base.StunTimer(seconds));
        StartCoroutine(base.StunTimer(seconds));
        yield break;
    }
}
