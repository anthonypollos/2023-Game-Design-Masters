using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyAttacks : EnemyAttackTemplate
{
    [SerializeField] float dashRange;
    [SerializeField] float dashTime;
    [SerializeField] [Tooltip("Chance at max dash range to trigger a dash")] 
    [Range(0, 1)]
    float baseDashChance;
    [SerializeField]
    [Tooltip("Chance to chose multi attack over single attack")]
    [Range(0, 1)]
    float multiAttackChance;
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
        float distance = Vector3.Distance(transform.position, brain.player.position);
        float chanceForDash = (distance / dashRange) * baseDashChance;
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
        brain.an.SetBool("Attacking", true);
        //Debug.Log("trigger attack" + attack);
        brain.an.SetTrigger("Attack" + attack.ToString());
        brain.state = EnemyStates.ATTACKING;
        brain.LookAtPlayer();
        jukebox.PlaySound(0);
    }

    public void Dashing()
    {
        brain.moveable.Dash(transform.forward * dashRange, dashTime);
    }

    public void AttackEnd()
    {
        count = 0;
        brain.state = EnemyStates.NOTHING;
        brain.an.SetBool("Attacking", false);
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
}
