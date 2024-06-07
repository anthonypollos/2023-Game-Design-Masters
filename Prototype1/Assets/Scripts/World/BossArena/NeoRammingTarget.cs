using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeoRammingTarget : MonoBehaviour, IDamageable, ITrap
{
    [SerializeField] NeoBossFightController bfm;
    [SerializeField] Animator an;

    private BossEnemyBrain brain;
    bool isDestroyed = false;
    [SerializeField] int health = 10;
    public void ActivateTrap(GameObject target)
    {
        //BossEnemyBrain check = target.GetComponent<BossEnemyBrain>();
        if (target == brain.gameObject)
        {
            OnDeath();
        }
    }

    private void Start()
    {
        brain = FindObjectOfType<BossEnemyBrain>(true);
    }

    public int GetHealth()
    {
        return 0;
    }

    public void TakeDamage(int dmg, DamageTypes damageType = DamageTypes.BLUGEONING)
    {
        health -= dmg;
        if (health <= 0) OnDeath();
    }

    public bool WillBreak(int dmg)
    {
        return true;
    }

    private void OnDeath()
    {
        if (!isDestroyed)
        {
            isDestroyed = true;
            an.SetTrigger("Hit");
            bfm.TargetHit();
            brain.Calm();
        }
    }
}
