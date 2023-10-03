using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyAttacks : EnemyAttackTemplate
{
    [SerializeField]
    [Tooltip("Shots till multishot")]
    int multiShotInterval = 3;
    int currentShot = 0;
    [SerializeField]
    [Tooltip("The projectile shot")]
    GameObject projectile;
    [SerializeField]
    [Tooltip("The location the projectiles spawn at")]
    Transform shootLocation;
    [SerializeField]
    [Tooltip("Projectile speed")]
    float projectileSpeed = 10f;
    [SerializeField]
    [Tooltip("Spread angle")]
    float spreadAngle = 10f;
    public override void Attack()
    {
        if (count >= attackSpeed)
        {
            count -= 0.5f;
            AttackAI();
        }
    }

    private void AttackAI()
    {
        TriggerAttack(1);
    }

    private void TriggerAttack(int attack)
    {
        brain.an.SetBool("Attacking", true);
        Debug.Log("trigger attack" + attack);
        brain.an.SetTrigger("Attack" + attack.ToString());
        brain.state = EnemyStates.ATTACKING;
        brain.LookAtPlayer();
    }


    public void AttackEnd()
    {
        count = 0;
        brain.state = EnemyStates.NOTHING;
        brain.an.SetBool("Attacking", false);
    }

    public void ShootProjectile()
    {
        List<Rigidbody> projectiles = new List<Rigidbody>();
        currentShot++;
        int projectilesToSpawn = currentShot == multiShotInterval ? 3 : 1;
        for (int i = 0; i < projectilesToSpawn; i++)
            projectiles.Add(Instantiate(projectile, shootLocation.position, Quaternion.identity).GetComponent<Rigidbody>());
        int temp = 0;
        foreach (Rigidbody projectile in projectiles)
        {
            temp++;
            switch(temp)
            {
                case 1:
                    projectile.AddForce(transform.forward * projectileSpeed, ForceMode.Impulse);
                    break;
                case 2:
                    Quaternion rotation1 = Quaternion.Euler(0, spreadAngle, 0);
                    projectile.AddForce(rotation1 * transform.forward * projectileSpeed, ForceMode.Impulse);
                    break;
                case 3:
                    Quaternion rotation2 = Quaternion.Euler(0, -spreadAngle, 0);
                    projectile.AddForce(rotation2 * transform.forward * projectileSpeed, ForceMode.Impulse);
                    break;
                default:
                    break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCounter();
    }

    protected override void UpdateCounter()
    {
        float previous = count;
        base.UpdateCounter();
        if (count <= previous) currentShot = 0;
    }
}
