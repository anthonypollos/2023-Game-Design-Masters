using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BruteEnemyInteractions : EnemyInteractionBehaviorTemplate
{
    [SerializeField]
    [Tooltip("Damage dealt and taken when colliding with someone then launched")]
    int clashDamage = 20;

    [Tooltip("A modifier for the amount of clash damage the brute will take when it runs into objects.\nDefault is 1. Below 1 makes it take less damage, above 1 makes it take more.")]
    [SerializeField] float dashClashModifier = 1;

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
        if(brain.moveable.isDashing)
            Break();
        else
        {
            brain.an.SetBool("Tendriled", true);
            Stunned();
            base.Lassoed();
        }
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
        jukebox.PlaySound(0);
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
    public void DashCollide()
    {
        StartCoroutine(Buffer());
    }

    public IEnumerator Buffer()
    {
        yield return new WaitForSeconds(0.1f);
        hasCollided = false;
    }

    public override void Death()
    {
        brain.health.Death();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(brain.moveable.isLaunched);
        Debug.Log(brain.moveable.isDashing);
        Debug.Log(collision.gameObject.name);
        if ((brain.moveable.isLaunched || brain.moveable.isDashing) && !collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Lasso") && !hasCollided)
        {
            hasCollided = true;
            GameObject hit = collision.gameObject;
            Debug.Log("Take clash damage");

            //If we were dashing, multiply the clash damage by the dash modifier and take that damage.
            if (brain.moveable.isDashing) brain.health.TakeDamage( (int)(clashDamage * dashClashModifier) );
            //otherwise, take the full clash damage.
            else brain.health.TakeDamage(clashDamage);

            IDamageable temp = hit.GetComponent<IDamageable>();
            if (temp != null)
            {
                temp.TakeDamage(clashDamage);
                jukebox.PlaySound(1);
            }

            ITrap temp2 = hit.GetComponent<ITrap>();
            if (temp2 != null)
            {
                Debug.Log("Take trap damage");
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
