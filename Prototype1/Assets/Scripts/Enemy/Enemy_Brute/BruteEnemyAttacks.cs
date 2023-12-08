using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BruteEnemyAttacks : EnemyAttackTemplate
{
    [Header("Quick Melee variables")]
    [SerializeField] float quickMeleeRange;
    [Header("Dashing variables")]
    [SerializeField] float dashRange;
    [SerializeField] float dashTime;
    [Header("JukeBox")]
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
        if (Vector3.Distance(brain.player.position, transform.position) < quickMeleeRange)
            TriggerAttack(1);
        else
            TriggerAttack(2);

    }

    private void TriggerAttack(int attack)
    {
        brain.an.SetFloat("AttackMod", 1);
        brain.an.SetBool("Attacking", true);
        //Debug.Log("trigger attack" + attack);
        currentWaitingTime = float.MaxValue;
        brain.state = EnemyStates.ATTACKING;
        brain.an.SetTrigger("Attack" + attack.ToString());
        brain.LookAtPlayer();
        jukebox.PlaySound(0);
        timeTest = Time.realtimeSinceStartup;
    }


    public void Dashing(int attack)
    {
        attack = attack - 1;
        if (attack < 0 && attack >= attackSeconds.Length)
        {
            Debug.LogError("Attack value for Dash invalid");
            return;
        }
        brain.moveable.Dash(transform.forward * dashRange, dashTime);
        currentWaitingTime = dashTime;
        if (animationTimer < 0)
            animationTimer = 0;
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
}
