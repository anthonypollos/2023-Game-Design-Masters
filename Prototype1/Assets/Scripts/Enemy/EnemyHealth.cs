using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FMODUnity;

//Josh Bonovich
//Controls and contains the enemies hp
public class EnemyHealth : MonoBehaviour, IDamageable
{
    //[SerializeField] private JukeBox jukebox;
    [SerializeField] protected int health = 100;
    [SerializeField] protected int staggerThreshold = 15;
    [SerializeField] protected GameObject bloodParticle;
    protected int maxHealth;
    [SerializeField] protected Slider healthSlider;
    //enemy container for controlling how many enemies are in the scene
    [HideInInspector]
    public EnemyContainer ec;
    [HideInInspector]
    public EnemyBrain brain;
    protected bool quitting = false;

    private OutlineToggle outlineManager;

    [SerializeField] protected EventReference enemyDamaged;
    [SerializeField] protected EventReference enemyDeath;

    [Tooltip("The particle object to spawn upon enemy death.\nNote: This can be any Game Object.")]
    [SerializeField] protected GameObject DeathParticle;
    //Set this here so that we never have a null case. It can be changed in-editor as needed
    [Tooltip("How long the Death Particle lives before Despawning.\nSet to 0 or below for an immortal object.")]
    [SerializeField] protected float DeathParticleLifetime = 4;

    protected bool dead = false;

    private void Awake()
    {
        outlineManager = FindObjectOfType<OutlineToggle>();
        quitting = false;
        //jukebox.SetTransform(transform);
        SceneManager.sceneUnloaded += OnSceneChange;
    }
    private void Start()
    {
        dead = false;
        ec = FindObjectOfType<EnemyContainer>();
        ec.AddEnemy(gameObject);
        maxHealth = health;
        if(healthSlider!=null)
            healthSlider.value = health / maxHealth;
    }
    private void Update()
    {
        if (transform.position.y < -20f)
            Die();
    }

    public virtual void TakeDamage(int dmg, DamageTypes damageType = DamageTypes.BLUGEONING)
    {
        health -= dmg;
        if (dmg > staggerThreshold)
        {
            AudioManager.instance.PlayOneShot(enemyDamaged, this.transform.position);
            //If there is a blood particle, create it.
            if (bloodParticle != null)
            {
                //create the particle
                GameObject vfxobj = Instantiate(bloodParticle, gameObject.transform.position, Quaternion.identity);
                //destroy the particle
                Destroy(vfxobj, 4);
            }

            brain.interaction.Stagger();
        }
        if (health <= 0) Die();
        if(healthSlider!=null)
            healthSlider.value = (float)health/ maxHealth;
        if (dmg > 0) //AudioManager.instance.PlayOneShot(enemyDamaged, this.transform.position);


        //Prevent overheal
        if (health > maxHealth) health = maxHealth;
    }

    public bool WillBreak(int dmg)
    {
        return (dmg >= health);
    }

    protected void Die()
    {
        if (!dead)
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.layer == 7)
                    collider.enabled = false;
            }
            StartCoroutine(WaitUntilStop(colliders));
        }
        
    }

    IEnumerator WaitUntilStop(Collider[] colliders)
    {
        dead = true;
        yield return new WaitUntil(() => brain.moveable.isDashing||!brain.moveable.isLaunched);
        foreach (Collider collider in colliders)
        {
            collider.gameObject.layer = 28;
        }
        brain.state = EnemyStates.DEAD;
        brain.moveable.ForceStop();
        brain.interaction.Death();
    }

    public void Death()
    {
        //jukebox.PlaySound(1);
        AudioManager.instance.PlayOneShot(enemyDeath, this.transform.position);
        ec.RemoveEnemy(gameObject);
        ec.RemoveAggro(gameObject);
        if (DeathParticle != null)
        {
            //print("Attempting to spawn " + DeathParticle + " from " + this + " at " + transform.position);
            GameObject tempparticle = Instantiate(DeathParticle, transform.position, Quaternion.identity);
            if (DeathParticleLifetime > 0) Destroy(tempparticle, DeathParticleLifetime);
        }
        Destroy(gameObject);
    }

    public int GetHealth()
    {
        return health;
    }

    private void OnEnable()
    {
        
    }


    private void OnDisable()
    {
        if (ec != null && !quitting)
        {
            ec.RemoveEnemy(gameObject, false);
            ec.RemoveAggro(gameObject);
        }
        Destroy(gameObject);
    }

    //checks if the unit is being destroyed via the scene unloading, this helps avoid problems with the save system
    private void OnApplicationQuit()
    {
        quitting = true;
    }

    private void OnSceneChange(Scene scene)
    {
        quitting = true;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    //forces a blood particle to spawn even if the stagger amount was not reached
    //BUG: If we force bleeding at the same time as regular bleed, this may result in a double-spawn!
    public void ForceSpawnBlood()
    {
        //If there is a blood particle, create it.
        if (bloodParticle != null)
        {
            //create the particle
            GameObject vfxobj = Instantiate(bloodParticle, gameObject.transform.position, Quaternion.identity);
            //destroy the particle
            Destroy(vfxobj, 4);
        }
    }

    private void OnDestroy()
    {
        foreach (Transform child in transform)
        {
            //If we have an outline, remove from the list on death
            if (child.GetComponent<Outline>() != null)
            {
                if(outlineManager != null)
                    outlineManager.RemoveOutline(child.gameObject);
            }
        }
    }
}
