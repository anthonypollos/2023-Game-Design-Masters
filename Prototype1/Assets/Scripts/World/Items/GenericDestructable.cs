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
        //Debug bc I'm *pretty* sure this def will force damageType to Blugeoning
        Debug.Log("Damage Type on " + this + " is " + damageType + "\nIf this is ALWAYS returning BLUGEONING, call a programmer");

        //Check for Immune Types. If we have none, run it.
        if (!immuneTypes.Contains(damageType) || (immuneTypes == null))
        {
            health -= damage;
            if (health <= 0) Destroy(gameObject);
        }
    }

    public int GetHealth()
    {
        return health;
    }

    public bool WillBreak(int dmg)
    {
        return (dmg >= health);
    }
}
