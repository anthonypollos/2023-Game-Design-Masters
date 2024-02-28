using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Josh Bonovich
//Controls and contains the enemies hp
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private JukeBox jukebox;
    [SerializeField] int health = 100;
    [SerializeField] int staggerThreshold = 15;
    [SerializeField] GameObject bloodParticle;
    int maxHealth;
    [SerializeField] Slider healthSlider;
    //enemy container for controlling how many enemies are in the scene
    [HideInInspector]
    public EnemyContainer ec;
    [HideInInspector]
    public EnemyBrain brain;
    bool quitting = false;

   [SerializeField] public bool canSpawnEnemies;
   [SerializeField] public int healthToSpawn;
   [SerializeField] public int enemiesToSpawn;
   [SerializeField] public float spawnRadius;
    public GameObject enemyToSpawn;

    private OutlineToggle outlineManager;

    private void Awake()
    {
        outlineManager = FindObjectOfType<OutlineToggle>();
        quitting = false;
        jukebox.SetTransform(transform);
        SceneManager.sceneUnloaded += OnSceneChange;
    }
    private void Start()
    {
        ec = FindObjectOfType<EnemyContainer>();
        ec.AddEnemy(gameObject);
        maxHealth = health;
        healthSlider.value = health / maxHealth;
    }
    private void Update()
    {
        if (transform.position.y < -20f)
            Die();
    }
    public void TakeDamage(int dmg, DamageTypes damageType = DamageTypes.BLUGEONING)
    {
        health -= dmg;
        if (dmg > staggerThreshold)
        {

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
        healthSlider.value = (float)health/ maxHealth;
        if (dmg > 0) jukebox.PlaySound(0);

        if (health <= healthToSpawn && canSpawnEnemies == true)
        {
            for (var i = 0; i < enemiesToSpawn; i++)
            {
                Vector2 dir = Random.insideUnitCircle * spawnRadius;

                dir = dir.normalized;

                Vector3 spawnLoc = transform.position;

                spawnLoc = spawnLoc + spawnRadius * (Vector3.right * dir.x + Vector3.forward * dir.y);
                Instantiate(enemyToSpawn, spawnLoc, Quaternion.identity);
                canSpawnEnemies = false;
            }
            
        }

        //Prevent overheal
        if (health > maxHealth) health = maxHealth;
    }

    public bool WillBreak(int dmg)
    {
        return (dmg >= health);
    }

    private void Die()
    {
        brain.state = EnemyStates.DEAD;
        brain.moveable.Hold();
        brain.interaction.Death();
    }

    public void Death()
    {
        jukebox.PlaySound(1);
        ec.RemoveEnemy(gameObject);
        ec.RemoveAggro(gameObject);
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
