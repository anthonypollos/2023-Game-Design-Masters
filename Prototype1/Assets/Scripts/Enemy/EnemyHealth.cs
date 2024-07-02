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

    [Header("Enemy HUD")]
    //[SerializeField] protected Slider healthSlider;
    [SerializeField] protected Image healthFill;

    [SerializeField] protected Slider fireSlider;
    [SerializeField] protected GameObject fireTimerUI;
    [SerializeField] protected Image fireTimerFill;

    [SerializeField] protected Slider bleedSlider;
    [SerializeField] protected GameObject bleedTimerUI;
    [SerializeField] protected Image bleedTimerFill;

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

    [Tooltip("Does this enemy ALWAYS stagger?")]
    [SerializeField] bool alwaysStagger = false;

    [Space(5)]

    [Header("Death Duration Overrides")]

    [SerializeField] bool useDeathDuration = false;
    [SerializeField] float deathDuration = 0f;

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
        //if (healthSlider != null) healthSlider.value = health / maxHealth;
        if (healthFill != null) healthFill.fillAmount = health / maxHealth;

        // Fire timer on spawn should be empty/disabled
        //if (fireSlider != null) fireSlider.value = 0;
        if (fireTimerUI != null) fireTimerUI.SetActive(false);

        // Same for bleed slider
        //if (bleedSlider != null) bleedSlider.value = 0;
        if (bleedTimerUI != null) bleedTimerUI.SetActive(false);
    }

    private void Update()
    {
        if (transform.position.y < -20f)
            Die();
    }

    public virtual void TakeDamage(int dmg, DamageTypes damageType = DamageTypes.BLUGEONING)
    {
        health -= dmg;
        if (alwaysStagger)
        {
            brain.an.SetTrigger("Damaged");
        }
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
        //if (healthSlider != null) healthSlider.value = (float)health/ maxHealth;
        if (healthFill != null) healthFill.fillAmount = (float)health / maxHealth;

        //If we have a fire slider and are taking fire damage, update the fire slider
        if (/*fireSlider != null*/ fireTimerUI != null && damageType == DamageTypes.FIRE)
        {
            if (fireTimerUI.activeInHierarchy != true)
                fireTimerUI.SetActive(true);

            fireTimerFill.fillAmount = (1 - (1 / (GetComponent<Flammable>().getDefaultEffectDuration() - GetComponent<Flammable>().getCurrentTime())));

            // should redo if keeping fire timer; for now, disables UI once close enough to 0
            if (fireTimerFill.fillAmount < 0.1f)
                fireTimerUI.SetActive(false);

            //fireSlider.value = (1 / (GetComponent<Flammable>().getDefaultEffectDuration() - GetComponent<Flammable>().getCurrentTime()));
        }
        //If we have a bleed slider and are taking bleed damage, update the bleed slider
        if (/*bleedSlider != null*/ bleedTimerUI != null && damageType == DamageTypes.BLEED)
        {
            if (bleedTimerUI.activeInHierarchy != true)
                bleedTimerUI.SetActive(true);

            bleedTimerFill.fillAmount = (1 - (1 / (GetComponent<Bleedable>().getDefaultEffectDuration() - GetComponent<Bleedable>().getCurrentTime())));

            // should redo if keeping bleed timer; for now, disables UI once close enough to 0
            if (bleedTimerFill.fillAmount < 0.1f)
                bleedTimerUI.SetActive(false);

            //bleedSlider.value = (1 / (GetComponent<Bleedable>().getDefaultEffectDuration() - GetComponent<Bleedable>().getCurrentTime()));
        }

        //DEPRECATED! Used to be used to play a sound upon damage before we switched to FMod
        //if (dmg > 0) //AudioManager.instance.PlayOneShot(enemyDamaged, this.transform.position);


        //Prevent overheal
        if (health > maxHealth) health = maxHealth;
    }

    public bool WillBreak(int dmg)
    {
        return (dmg >= health);
    }

    public int GetStaggerValue()
    {
        return staggerThreshold;
    }

    protected virtual void Die()
    {
        if (!dead)
        {
            dead = true;
            brain.state = EnemyStates.DEAD;
            brain.an.SetTrigger("Damaged");
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.layer == 7)
                    collider.enabled = false;
                else
                    collider.gameObject.layer = LayerMask.NameToLayer("Dead");
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

        //If we have "useDeathDuration" turned on, we do not want the death to be handled by an Anim Event.
        //Consequently, we run the "Death" command in here instead.
        if (useDeathDuration)
        {
            //First, if we DO have a model that's disconnected from enemy, set the trigger on said model to play the Death anim
            brain.an.SetTrigger("Death");
            //Second, we call the Death void in here directly
            StartCoroutine(DelayDeath());
        }
        else brain.interaction.Death();
    }

    public void Death()
    {
        //jukebox.PlaySound(1);
        AudioManager.instance.PlayOneShot(enemyDeath, this.transform.position);
        ec.RemoveEnemy(gameObject);
        ec.RemoveAggro(gameObject);
        if (DeathParticle != null)
        {
            GameObject tempparticle = Instantiate(DeathParticle, transform.position, Quaternion.identity);
            if (DeathParticleLifetime > 0) Destroy(tempparticle, DeathParticleLifetime);
        }
         Destroy(gameObject);
    }

    /// <summary>
    /// Override coroutine that waits a set amount of time before destroying the object
    /// We need to use this in case we have an "enemy" whose visible model cannot be tied to the enemy itself
    /// I am implementing this specifically for the town square cysts, since their models are complex and have colliders not used for getting damaged.
    /// Conseqeuently, the actual cyst "enemy" is an invisible object inside of the cyst model.
    /// This is used so that it's not officially "dead" until the animation is done.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayDeath()
    {
        yield return new WaitForSeconds(deathDuration);
        Death();
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

    public Slider GetFireSlider()
    {
        return fireSlider;
    }

    public void ClearFireSlider()
    {
        fireSlider.value = 0;
    }

    public Slider GetBleedSlider()
    {
        return bleedSlider;
    }

    public void ClearBleedSlider()
    {
        bleedSlider.value = 0;
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
