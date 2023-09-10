using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] int health = 100;


    public void TakeDamage(int dmg)
    {
        //health -= dmg;
        //if (health <= 0) Die();
    }

    private void Die()
    {
    }
}
