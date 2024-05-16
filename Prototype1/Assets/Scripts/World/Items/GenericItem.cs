using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FMODUnity;

public class GenericItem : MonoBehaviour, IKickable, IPullable, IDamageable
{
    [SerializeField] DamageTypes[] immuneTypes;
    private Moveable moveable;
    //this is a private internal var for constraints that we need if the object is frozen before being tendrilled
    private RigidbodyConstraints rbconstraints;
    [Tooltip("Enable if this object should stay in place before being grabbed.")]
    [SerializeField] bool _frozenBeforeTendril = false;
    [Tooltip("Set to 0 or lower for max health to be near infinite")]
    [SerializeField] private int maxHealth = 20;
    [Tooltip("The object that's created when this item is destroyed.\nDoes not need to be a particle.")]
    [SerializeField] GameObject DestructionParticle;
    [Tooltip("The amount of time the Destruction Particle Object is alive for.\n0 means it lives forever.")]
    [SerializeField] float DestructionParticleLifetime = 4f;
    [SerializeField] GameObject KickedParticle;
    [SerializeField][Tooltip("Add the name of the status you want to invoke when this object hits a target")] string[] effectsOnHit;

    //[SerializeField] private JukeBox jukebox;
    [SerializeField] private EventReference collideSound;
    [SerializeField] private EventReference breakSound;
    //Keeps track of if this object is alive or not. This is to prevent it from double dying.
    private bool isAlive = true;
    private int health;

    bool wasOn = false;
    [SerializeField] bool respawning = false;
    [SerializeField] float respawnDelay = 5f;
    Vector3 initialPos;
    Quaternion initialRot;

    private OutlineToggle outlineManager;
    //This boolean will keep track of whether or not the object was, at any point, frozen, even IF it is no longer frozen.
    private bool wasFrozen;
    //This float stores the mass of the object if frozenBeforeTendril is true.
    private float realMass;

    private void Awake()
    {
        if (maxHealth <= 0)
            maxHealth = int.MaxValue;
        //jukebox.SetTransform(transform);
        outlineManager = FindObjectOfType<OutlineToggle>();
    }
    private void OnEnable()
    {
        if (maxHealth <= 0)
            maxHealth = int.MaxValue;
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
            //Moved to "Freeze" so that respawned objects can have this
            Freeze();
        }
        //maxHealth = health;
    }

    /// <summary>
    /// Freeze the object. We commit this to its own public thing so that it can be caught by GameController.cs and used with respawning objects.
    /// </summary>
    public void Freeze()
    {
        //Get the constraints and assign them to rbconstraints
        //We will NEED this if we ever want to have objects that only move along single axes (like the minecart)
        //if check to make sure that the past constraints weren't "nothing." If they ARE nothing, assume this is a respawned object.
        if (rbconstraints != RigidbodyConstraints.None) rbconstraints = GetComponent<Rigidbody>().constraints;
        //We have no way of accessing frozenBeforeTendril on a respawned object prior to now, so we set it to true *in* this function.
        //This won't matter for first-time spawned objects, but for respawning objects, we need to be able to see that this is set to true so that every new one is frozen.
        _frozenBeforeTendril = true;
        //Now freeze.
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //get the actual mass of the object
        realMass = GetComponent<Rigidbody>().mass;
        //Set the mass to 1000 so that it becomes solid to the player
        GetComponent<Rigidbody>().mass = 1000f;
        //We were (and are currently) frozen, make sure this bool is set to true
        wasFrozen = true;
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

    public void Pulled(IsoAttackManager player = null)
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

    public int GetHealth()
    {
        return maxHealth;
    }

    public void TakeDamage(int dmg, DamageTypes damageType = DamageTypes.BLUGEONING)
    {
        if (!immuneTypes.Contains(damageType))
        {
            health -= dmg;
            //jukebox.PlaySound(0);
            AudioManager.instance.PlayOneShot(collideSound, this.transform.position);
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
                            //ALSO: If the particles have outlines, add them to the outline manager.
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
                                //Check the children of the gib object. Find whatever child has the outline and add it
                                if (child.GetComponent<Outline>() != null)
                                {
                                    outlineManager.AddOutline(child.gameObject);
                                    //de-activate the outline
                                    child.GetComponent<Outline>().enabled = false;
                                }
                            }
                        }
                        //jukebox.PlaySound(0);
                        //jukebox.PlaySound(1);
                        AudioManager.instance.PlayOneShot(collideSound, this.transform.position);
                        AudioManager.instance.PlayOneShot(breakSound, this.transform.position);
                        gameObject.SetActive(false);
                    }
                }
            //Prevent overheal
            if (health > maxHealth) health = maxHealth;
        }
    }

    public bool WillBreak(int dmg)
    {
        return (dmg >= health);
    }

    /// <summary>
    /// Public method to set this object's Rigid Body Constraints.
    /// I committed this to a public method because, for respawning objects that start frozen in place, we need to get the constraints BEFORE we freeze it.
    /// This might be messy or unneeded, but if it gets the job done, it gets the job done, and perf is fine right now anyway.
    /// </summary>
    public void SetConstraints()
    {
        GetComponent<Rigidbody>().constraints = rbconstraints;
    }
    /// <summary>
    /// Fuck it, if Set Constraints isn't doing what I want, we make a default constraint list of... no constraints and set respawned objects to THAT.
    /// Just don't have any respawning objects with constraints on them for right now.
    /// </summary>
    public void SetDefaultConstraints()
    {
        rbconstraints = RigidbodyConstraints.None;
        print("Test:" + rbconstraints);
        GetComponent<Rigidbody>().constraints = rbconstraints;
    }

    private void Unfreeze()
    {
        //1. reset the object's mass to whatever its original value was
        GetComponent<Rigidbody>().mass = realMass;
        //2. return to non-frozen constraints taken from start
        SetConstraints();
        //3. turn the boolean off so this is only called once.
        _frozenBeforeTendril = false;
        //4. just in case it wasn't already set, set wasFrozen to true;
        wasFrozen = true;
    }

    private void OnDisable()
    {
        if (wasOn)
        {
            wasOn = false;
            if (respawning)
            {
                //We use the second overload for this now so that it knows whether or not the original was frozen
                GameController.instance.Respawn(gameObject, respawnDelay, initialPos, initialRot, wasFrozen);
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
