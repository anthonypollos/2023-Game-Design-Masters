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
    [Tooltip("Spread angle")]
    float spreadAngle = 10f;

    [SerializeField] private JukeBox jukebox;

    private void Awake()
    {
        jukebox.SetTransform(transform);
    }
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
        brain.state = EnemyStates.ATTACKING;
        brain.an.SetFloat("AttackMod", 1);
        //Debug.Log("attack triggered");
        brain.an.SetBool("Attacking", true);
        currentWaitingTime = float.MaxValue;
        //Debug.Log("trigger attack" + attack);
        brain.an.SetTrigger("Attack" + attack.ToString());
        //brain.state = EnemyStates.ATTACKING;
        brain.LookAtPlayer();
    }


  

    public void ShootProjectile()
    {
        currentShot++;
        int projectilesToSpawn = (currentShot%multiShotInterval)==0 ? 3 : 1;
        jukebox.PlaySound(1);
        //if (currentShot%multiShotInterval == 0) Debug.Log("trigger multi");
        for (int i = 0; i < projectilesToSpawn; i++)
        { 
            IProjectile shot = Instantiate(projectile, shootLocation.position, Quaternion.identity).GetComponent<IProjectile>();
            switch (i)
            {
                case 0:
                    shot.Shoot(transform.forward, brain.player.position);
                    break;
                case 1:
                    Quaternion rotation1 = Quaternion.Euler(0, spreadAngle, 0);
                    shot.Shoot(rotation1 * transform.forward, brain.player.position);
                    break;
                case 2:
                    Quaternion rotation2 = Quaternion.Euler(0, -spreadAngle, 0);
                    shot.Shoot(rotation2 * transform.forward, brain.player.position);
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
        animationTimer = float.MinValue;
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
        if (count < previous) currentShot = 0;
    }
}
