using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericDestructable : MonoBehaviour, IDamageable
{

    [SerializeField] private int health = 20;
    public void TakeDamage(int damage)
    {
        health-= damage; 
        if (health <= 0) Destroy(gameObject);
    }

    public bool WillBreak(int dmg)
    {
        return (dmg >= health);
    }
}
