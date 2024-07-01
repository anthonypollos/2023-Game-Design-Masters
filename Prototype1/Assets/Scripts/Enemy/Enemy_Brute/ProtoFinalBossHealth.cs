using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FMODUnity;


/// <summary>
/// A duplicate of EnemyHealth circa 5/16/24 that includes threshhold spawning
/// made with the intent to test the final boss stuff
/// Sean Lee
/// </summary>

public class ProtoFinalBossHealth : EnemyHealth
{


    [Header("Final Boss Settings")]

    //temp final boss health threshhold test thingy
    [SerializeField] public bool bossThreshholds;

    //Private health check ints for final boss threshholds.
    //This will be set at runtime by finding the percentages of the boss's max health
    private int threshhold0; //90%
    private int threshhold1; //75%
    private int threshhold2; //50%
    private int threshhold3; //25%

    private bool threshhold0Spawned;
    private bool threshhold1Spawned;
    private bool threshhold2Spawned;
    private bool threshhold3Spawned;

    [SerializeField] GameObject threshold0Object; //object to spawn at 90% health
    [SerializeField] GameObject threshold1Object; //object to spawn at 75% health
    [SerializeField] GameObject threshold2Object; //object to spawn at 50% health
    [SerializeField] GameObject threshold3Object; //object to spawn at 25% health

    public Vector3 ArenaLocation;

    private void Start()
    {
        dead = false;
        ec = FindObjectOfType<EnemyContainer>();
        ec.AddEnemy(gameObject);
        maxHealth = health;
        //healthSlider.value = health / maxHealth;
        healthFill.fillAmount = health / maxHealth;

        //If this is the proto final boss, set the health threshholds
        if (bossThreshholds)
        {
            threshhold0 = (int)(maxHealth * 0.9);
            threshhold1 = (int)(maxHealth * 0.75);
            threshhold2 = (int)(maxHealth * 0.5);
            threshhold3 = (int)(maxHealth * 0.25);
            //now set the spawned bools to false. We use these to make sure threshhold gameobjects are only ever spawned once
            threshhold0Spawned = false;
            threshhold1Spawned = false;
            threshhold2Spawned = false;
            threshhold3Spawned = false;
            print("Threshholds: " + threshhold0 + " " + threshhold1 + " " + threshhold2 + " " + threshhold3);
        }
    }

    public override void TakeDamage(int dmg, DamageTypes damageType = DamageTypes.BLUGEONING)
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
        //healthSlider.value = (float)health / maxHealth;
        healthFill.fillAmount = (float)health / maxHealth;
        if (dmg > 0) //AudioManager.instance.PlayOneShot(enemyDamaged, this.transform.position);

        //if this is the proto final boss, run the checks for its health threshholds
        if (bossThreshholds) CheckThreshholds();

        //Prevent overheal
        if (health > maxHealth) health = maxHealth;
    }

    private void CheckThreshholds()
    {
        if ((health <= threshhold0) && (threshhold0Spawned == false))
        {
            threshhold0Spawned = true;
            Instantiate(threshold0Object, ArenaLocation, Quaternion.identity);
        }
        if ((health <= threshhold1) && (threshhold1Spawned == false))
        {
            threshhold1Spawned = true;
            Instantiate(threshold1Object, ArenaLocation, Quaternion.identity);
        }
        if ((health <= threshhold2) && (threshhold2Spawned == false))
        {
            threshhold2Spawned = true;
            Instantiate(threshold2Object, ArenaLocation, Quaternion.identity);
        }
        if ((health <= threshhold3) && (threshhold3Spawned == false))
        {
            threshhold3Spawned = true;
            Instantiate(threshold3Object, ArenaLocation, Quaternion.identity);
        }
    }

}
