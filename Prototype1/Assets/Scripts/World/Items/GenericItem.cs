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
    [SerializeField] private int maxHealth = 20;
    [Tooltip("The object that's created when this item is destroyed.\nDoes not need to be a particle.")]
    [SerializeField] GameObject DestructionParticle;
    [Tooltip("The amount of time the Destruction Particle Object is alive for.\n0 means it lives forever.")]
    [SerializeField] float DestructionParticleLifetime = 4f;
    [SerializeField] GameObject KickedParticle;
    [SerializeField][Tooltip("Add the name of the status you want to invoke when this object hits a target")] string[] effectsOnHit;

    [SerializeField] private JukeBox jukebox;

    //Keeps track of if this object is alive or not. This is to prevent it from double dying.
    private bool isAlive = true;
    private int health;

    bool wasOn = false;
    [SerializeField] bool respawning = false;
    [SerializeField] float respawnDelay = 5f;
    Vector3 initialPos;
    Quaternion initialRot;

    private OutlineToggle outlineManager;


    private void Awake()
    {
        jukebox.SetTransform(transform);
        outlineManager = FindObjectOfType<OutlineToggle>();
    }
    private void OnEnable()
    {
        health = maxHealth;
        wasOn = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
        initialRot = transform.rotation;
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
        //maxHealth = health;
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
        if (dmg > 0)
        if (health <= 0)
        {
            //Bool to prevent double death
            if (isAlive)
            {
                DestroyEvent();
                //First thing's first, turn isAlive off so this can't run again
                isAlive = false;
                //If there is a destruction particle, create it.
                if (DestructionParticle != null)
                {
                    //create the particle, gib, etc.
                    GameObject vfxobj = Instantiate(DestructionParticle, gameObject.transform.position, Quaternion.identity);
                    //Check to see if the particle has a lifetime.
                    if (DestructionParticleLifetime != 0f)
                    {
                        //destroy the particle
                        Destroy(vfxobj, DestructionParticleLifetime);
                    }

                    //This code makes gibs inherit their parent's angles and velocity
                    foreach (Transform child in vfxobj.transform)
                    {
                        //check every child in the instantiated object to see if they have rigidbodies.
                        if (child.gameObject.GetComponent<Rigidbody>() != null)
                        {
                            //If the child has a rigidbody, set its angles to the object that's breaking's angles
                            child.eulerAngles = GetComponent<Transform>().eulerAngles;

                            //Now set the velocity to the object that's breaking's velocity
                            child.gameObject.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;

                            //If this is an explosion, increase the velocity ten-fold
                            if (vfxobj.CompareTag("Explosion")) child.gameObject.GetComponent<Rigidbody>().velocity *= 10;
                        }
                    }
                }
                jukebox.PlaySound(0);
                jukebox.PlaySound(1);
                gameObject.SetActive(false);
            }
        }
        //Prevent overheal
        if (health > maxHealth) health = maxHealth;
    }

    public bool WillBreak(int dmg)
    {
        return (dmg >= health);
    }



    private void Unfreeze()
    {
        //1. return to non-frozen constraints taken from start
        GetComponent<Rigidbody>().constraints = rbconstraints;
        //2. turn the boolean off so this is only called once.
        _frozenBeforeTendril = false;
    }

    public int GetHealth()
    {
        return health;
    }

    private void OnDisable()
    {
        if (wasOn)
        {
            wasOn = false;
            if (respawning)
            {
                GameController.instance.Respawn(gameObject, respawnDelay, initialPos, initialRot);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void OnHitModifier(GameObject target)
    {
        foreach (string effect in effectsOnHit)
        {
            switch (effect)
            {
                case "Flammable":
                    Flammable flammable;
                    if (target.TryGetComponent<Flammable>(out flammable))
                        flammable.Activate();
                    break;
                case "Bleedable":
                    Bleedable bleedable;
                    if (target.TryGetComponent<Bleedable>(out bleedable))
                        bleedable.Activate();
                    break;
                default:
                    Debug.Log("Effect " + effect + " isn't implemented or is spelled wrong");
                    break;
            }
        }
    }
    public int GetMaxHealth()
    {
        return maxHealth;
    }

    private void DestroyEvent()
    {
        if (outlineManager != null)
        {
            foreach (Transform child in transform)
            {
                //If we have an outline, remove from the list on death
                if (child.GetComponent<Outline>() != null)
                {
                    outlineManager.RemoveOutline(child.gameObject);
                }
            }
        }
    }
}
