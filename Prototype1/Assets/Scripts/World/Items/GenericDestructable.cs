using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenericDestructable : MonoBehaviour, IDamageable
{
    [SerializeField] DamageTypes[] immuneTypes;
    [SerializeField] private int health = 20;
    public void TakeDamage(int damage, DamageTypes damageType = DamageTypes.BLUGEONING)
    {
        if (!immuneTypes.Contains(damageType))
        {
            health -= damage;
            if (health <= 0) Destroy(gameObject);
        }
    }

    public int GetHealth()
    {
        return 0;
    }

    public bool WillBreak(int dmg)
    {
        return (dmg >= health);
    }
}
