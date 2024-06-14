using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeoRammingTarget : MonoBehaviour, IDamageable, ITrap
{
    [SerializeField] NeoBossFightController bfm;
    [SerializeField] Animator an;

    private BossEnemyBrain brain;
    bool isDestroyed = false;

    //We serialize the organ object so that we can set its anims and stuff
    [SerializeField] GameObject organ;
    [SerializeField] int health = 10;

    private int twothirds;
    private int onethird;
    private Animator organAnim;
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
        twothirds = health * (2 / 3);
        onethird = health / 3;
        organAnim = organ.GetComponent<Animator>();
    }

    public int GetHealth()
    {
        return 0;
    }

    public void TakeDamage(int dmg, DamageTypes damageType = DamageTypes.BLUGEONING)
    {
        health -= dmg;


        //TODO: Can this be a Switch-Case or is this fine as nested ifs?

        //No more HP, die
        if (health <= 0)
        {
            OnDeath();
        }
        //We DO have an organ with an animator
        if (organAnim != null)
        {
            //Still alive, play the hit animation
            if (health > 0)
            {
                organAnim.SetTrigger("Hit");
            }
            //two thirds health, start Idle 2
            if (health <= twothirds)
            {
                organAnim.SetInteger("health", 2);
            }
            //last third, start Idle 3
            if (health <= onethird)
            {
                organAnim.SetInteger("health", 1);
            }
        }
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
            if (organAnim != null) organ.GetComponent<Animator>().SetTrigger("Die");
            bfm.TargetHit();
            brain.Calm();
        }
    }
}
