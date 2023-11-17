using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private JukeBox jukebox;
    [SerializeField] int health = 100;
    [SerializeField] int staggerThreshold = 15;
    [SerializeField] GameObject bloodParticle;
    int maxHealth;
    [SerializeField] Slider healthSlider;
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
        //slider = GetComponentInChildren<Slider>();
        healthSlider.value = health / maxHealth;
    }
    private void Update()
    {
        if (transform.position.y < -20f)
            Die();
    }
    public void TakeDamage(int dmg)
    {
        //Debug.Log("dealt damage");
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
        //Debug.Log((float)health / maxHealth);
        //Debug.Log(health);
        healthSlider.value = (float)health/ maxHealth;
        jukebox.PlaySound(0);
    }

    private void Die()
    {
        brain.state = EnemyStates.DEAD;
        //brain.interaction.Stagger();
        brain.interaction.Death();
    }

    public void Death()
    {
        jukebox.PlaySound(1);
        ec.RemoveEnemy(gameObject);
        ec.RemoveAggro(gameObject);
        Destroy(gameObject);
    }


    private void OnDisable()
    {
        if (ec != null && !quitting)
        {
            ec.RemoveEnemy(gameObject);
            ec.RemoveAggro(gameObject);
        }
        Destroy(gameObject);
    }

    private void OnApplicationQuit()
    {
        quitting = true;
    }

    private void OnSceneChange(Scene scene)
    {
        quitting = true;
    }
}
