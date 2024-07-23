using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyAttacks : EnemyAttackTemplate
{
    [HideInInspector]
    public BossEnemyBrain bossBrain;
    [Header("Boss Values")]
    [SerializeField] [Tooltip("Effective CD")]float timeToStartCharging;
    [SerializeField] int damageToPlayerOnHit;
    [SerializeField] float timeToEndCharging;
    [SerializeField]
    [Tooltip("Effective CD")]
    float timeToStartRam;
    Rigidbody rb;
    Collider myCollider;
    bool playerInvulnerable = false;
    [SerializeField]
    [Tooltip("Time player is imune to damage from charge when hit. Still effected by knockback")]
    float playerInvulTime = 1;
    bool isRamming = false;

    [Header("Dashing variables")]
    [SerializeField] float dashRange;
    [SerializeField] float dashTime;

    [Header("Audio")]
    [SerializeField] private EventReference enemyAggro;
    [SerializeField] private EventReference enemyAttack;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        myCollider = rb.GetComponent<Collider>();
        count = 0;
        animationTimer = float.MinValue;
    }

    private void Update()
    {
        UpdateCounter();
    }
    public override void Attack()
    {
        if(bossBrain.state == EnemyStates.NOTHING && count > timeToStartCharging)
        {
            count = 0;
            bossBrain.state = EnemyStates.ATTACKING;
            //play animation here that would then call Charge();
            Charge();
        }
        else if (bossBrain.state == EnemyStates.ENRAGED && count > timeToStartRam && !isRamming)
        {
            isRamming = true;
            count = 0;
            TriggerAttack(2);
        }
        
    }

    public void Charge()
    {
        bossBrain.state = EnemyStates.CHARGING;
        StartCoroutine(ChargingDuration());
    }

    IEnumerator ChargingDuration()
    {
        float time = 0;

        while (time < timeToEndCharging && bossBrain.state == EnemyStates.CHARGING)
        {
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
            count = 0;
        }
        count = 0;
        if (bossBrain.state == EnemyStates.CHARGING)
            bossBrain.state = EnemyStates.NOTHING;
        count = 0;

    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Boss hit target");
        if(collision.gameObject.CompareTag("Player") && bossBrain.state == EnemyStates.CHARGING)
        {
            Debug.Log("Conditions Met");
            
            Vector3 dir = collision.transform.position - collision.contacts[0].point;
            dir.y = 0;
            dir = dir.normalized;
            collision.gameObject.GetComponent<Moveable>().Slammed(dir, rb.mass * brain.moveable.GetSpeed(), myCollider, 0);
            if (!playerInvulnerable)
            {
                collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damageToPlayerOnHit);
                StartCoroutine(InvulnerabilityFrames());
            }
        }
    }

    IEnumerator InvulnerabilityFrames()
    {
        playerInvulnerable = true;
        yield return new WaitForSeconds(playerInvulTime);
        playerInvulnerable = false;
    }

    public void TriggerAttack(int attack)
    {
        brain.an.SetFloat("AttackMod", 1);
        brain.an.SetBool("Attacking", true);
        currentWaitingTime = float.MaxValue;
        //brain.state = EnemyStates.ATTACKING;
        brain.an.SetTrigger("Attack" + attack.ToString());
        brain.LookAtPlayer();
        //jukebox.PlaySound(0);
        AudioManager.instance.PlayOneShot(enemyAggro, this.transform.position);
        timeTest = Time.realtimeSinceStartup;
    }


    public void Dashing(int attack)
    {
        windUp = false;
        attack = attack - 1;
        if (attack < 0 && attack >= attackSeconds.Length)
        {
            Debug.LogError("Attack value for Dash invalid");
            return;
        }
        brain.moveable.Ram(transform.forward * dashRange, dashTime);
        //jukebox.PlaySound(1);
        AudioManager.instance.PlayOneShot(enemyAttack, this.transform.position);
        currentWaitingTime = dashTime;
        if (animationTimer < 0)
            animationTimer = 0;
    }

    public void Enrage()
    {
        animationTimer = float.MinValue;
        count = 0;
    }

    public override void AttackEnd()
    {
        
        base.AttackEnd();
        isRamming = false;

        brain.an.SetFloat("MoveState", 0);
    }

}
