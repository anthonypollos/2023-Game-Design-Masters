using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable 
{
    
    public void TakeDamage(int dmg, DamageTypes damageType = DamageTypes.BLUGEONING);
    public bool WillBreak(int dmg);
}
