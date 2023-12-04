using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericItem : MonoBehaviour, IKickable, IPullable, IDamageable
{
    private Moveable moveable;
    //this is a private internal var for constraints that we need if the object is frozen before being tendrilled
    private RigidbodyConstraints rbconstraints;
    [Tooltip("Enable if this object should stay in place before being grabbed.")]
    [SerializeField] bool _frozenBeforeTendril = false;
    [SerializeField] private int health = 20;
    [SerializeField] int clashDamage;
    [Tooltip("The object that's created when this item is destroyed.\nDoes not need to be a particle.")]
    [SerializeField] GameObject DestructionParticle;
    [Tooltip("The amount of time the Destruction Particle Object is alive for.\n0 means it lives forever.")]
    [SerializeField] float DestructionParticleLifetime = 4f;
    [SerializeField] GameObject KickedParticle;

    [SerializeField] private JukeBox jukebox;

    private void Awake()
    {
        jukebox.SetTransform(transform);
    }
    // Start is called before the first frame update
    void Start()
    {
        moveable = GetComponent<Moveable>();
        //If set to be frozen, freeze.
        if (_frozenBeforeTendril)
        {
            //Get the constraints and assign them to rbconstraints
            //We will NEED this if we ever want to have objects that only move along single axes (like the minecart)
            rbconstraints = GetComponent<Rigidbody>().constraints;
            //Now freeze.
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void Kicked()
    {
        //If there is a kicked particle, create it.
        if (KickedParticle != null)
        {
            //create the particle
            GameObject vfxobj = Instantiate(KickedParticle, gameObject.transform.position, Quaternion.identity);
            //destroy the particle
            Destroy(vfxobj, 4);
        }
        //unfreeze, then disable frozenbeforetendril so this isn't called every time.
        if (_frozenBeforeTendril)
        {
            Unfreeze();
        }
    }

    public void Pulled()
    {
        //unfreeze, then disable frozenbeforetendril so this isn't called every time.
        if (_frozenBeforeTendril)
        {
            Unfreeze();
        }
    }

    public void Lassoed()
    {
        
    }

    public void Break()
    {

    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        jukebox.PlaySound(0);
        if (health <= 0)
        {
            //If there is a destruction particle, create it.
            if (DestructionParticle != null)
            {
                //create the particle
                GameObject vfxobj = Instantiate(DestructionParticle, gameObject.transform.position, Quaternion.identity);
                //Check to see if the particle has a lifetime.
                if (DestructionParticleLifetime != 0f)
                {
                    //destroy the particle
                    Destroy(vfxobj, DestructionParticleLifetime);
                }
            }
            jukebox.PlaySound(1);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (moveable != null)
        {
            if (!collision.gameObject.CompareTag("Player") && moveable.isLaunched)
            {
                ITrap trap = collision.gameObject.GetComponent<ITrap>();
                if (trap != null)
                {
                    trap.ActivateTrap(gameObject);
                }

                if (!collision.gameObject.CompareTag("Ground"))
                {
                    //Debug.Log("Deal damage to hit target");
                    IDamageable dam = collision.gameObject.GetComponent<IDamageable>();
                    if (dam != null)
                        dam.TakeDamage(clashDamage);
                    TakeDamage(clashDamage);
                }
            }
        }

    }

    private void Unfreeze()
    {
        //1. return to non-frozen constraints taken from start
        GetComponent<Rigidbody>().constraints = rbconstraints;
        //2. turn the boolean off so this is only called once.
        _frozenBeforeTendril = false;
    }

}
