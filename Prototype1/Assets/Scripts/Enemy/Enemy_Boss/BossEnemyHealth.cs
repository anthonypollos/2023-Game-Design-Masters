using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyHealth : EnemyHealth
{
    [HideInInspector]
    public BossEnemyBrain bossBrain;
    [SerializeField] int[] healthThresholds;
    int healthThreshold = 0;

    public override void TakeDamage(int dmg, DamageTypes damageType = DamageTypes.BLUGEONING)
    {
        if (brain.state == EnemyStates.ENRAGED) return;
        if(healthThreshold<healthThresholds.Length && health-dmg <= healthThresholds[healthThreshold])
        {
            ToggleThreshold();
        }
        else
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
        }
        if (health <= 0) Die();
        healthSlider.value = (float)health / maxHealth;
        //if (dmg > 0) //AudioManager.instance.PlayOneShot(enemyDamaged, this.transform.position);

        //Prevent overheal
        if (health > maxHealth) health = maxHealth;
    }

    private void ToggleThreshold()
    {
        bossBrain.Enrage();
        health = healthThresholds[healthThreshold];
        healthThreshold++;
        
    }
}
