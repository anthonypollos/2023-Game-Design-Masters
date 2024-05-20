using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyAttacks : EnemyAttackTemplate
{
   // [SerializeField] float dashRange;
    //[SerializeField] float dashTime;
    [Header("Attack Chances")]
    [SerializeField] [Tooltip("Chance at max dash range to trigger a dash")] 
    [Range(0, 1)]
    float baseDashChance;
    [SerializeField]
    [Tooltip("Chance to chose multi attack over single attack")]
    [Range(0, 1)]
    float multiAttackChance;
    [Header("Dashing variables")]
    [SerializeField] float[] dashRanges;
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
            count = attackSpeed - 0.5f;
            AttackAI();
        }
    }

    private void AttackAI()
    {
        float distance = Vector3.Distance(transform.position, brain.player.position);
        float chanceForDash = (distance / dashRanges[2]) * baseDashChance;
        if(distance>minAttackRange)
        {
            float roll = Random.Range(0f, 1f);
            if (roll <= chanceForDash)
                TriggerAttack(3);
        }
        else
        {
            float roll = Random.Range(0f, 1f);
            if (roll > multiAttackChance)
                TriggerAttack(1);
            else
                TriggerAttack(2);
        }
        
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
        float mod = attack == 1 ? (1f / 3) : 1;
        brain.moveable.Dash(transform.forward * dashRanges[attack] * mod, attackSeconds[attack] * mod);
    }

    public void AttackEnd()
    {
        count = 0;
        brain.state = EnemyStates.NOTHING;

        brain.an.SetBool("Attacking", false);
        Debug.Log(Time.realtimeSinceStartup - timeTest);
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
