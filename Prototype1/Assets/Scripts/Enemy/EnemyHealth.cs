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

    private void Awake()
    {
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
    public void TakeDamage(int dmg)
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
        jukebox.PlaySound(0);
    }

    public bool WillBreak(int dmg)
    {
        return (dmg >= health);
    }

    private void Die()
    {
        brain.state = EnemyStates.DEAD;
        brain.interaction.Death();
    }

    public void Death()
    {
        jukebox.PlaySound(1);
        ec.RemoveEnemy(gameObject);
        ec.RemoveAggro(gameObject);
        Destroy(gameObject);
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
}
