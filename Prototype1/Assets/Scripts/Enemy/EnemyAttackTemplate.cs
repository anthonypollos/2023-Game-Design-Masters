using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Josh Bonovich
//This is the abstrac template for all enemy attacks

public abstract class EnemyAttackTemplate : MonoBehaviour
{
    //Data on how an attack call will go
    [Header("Starting attack data")]
    [SerializeField] 
    [Tooltip("Range at which the enemy can start attacking")]
    public float maxAttackRange;
    [SerializeField]
    [Tooltip("Range at which the enemy will always do an attack")]
    protected float minAttackRange;
    [SerializeField]
    [Tooltip("Time it takes once in range to attack (NOT USED ON BOSS)")]
    public float attackSpeed;
    [HideInInspector]
    public EnemyBrain brain;

    //Amount of seconds an attack actually takes to wind up animation wise
    [Header("Attack timing information")]
    [SerializeField] protected float[] attackWindUpSeconds;
    [SerializeField] protected float[] attackSeconds;
    [SerializeField] protected float[] attackWindDownSeconds;

    //frame data for attacks
    [Header("Attack frame data")]
    [SerializeField] protected int[] attackFrames;
    [SerializeField] protected int attackFramesPerSecond;
    protected bool windUp = false;
    

    protected float timeTest;
    public abstract void Attack();

    protected float count = 0;

    [SerializeField] protected float animationTimer = float.MinValue;

    protected float currentWaitingTime = float.MaxValue;

    //Timers getting updated through the parents update method
    protected virtual void UpdateCounter()
    {
        if (brain.interaction.stunned)
        {
            windUp = false;
            count = 0;
        }
        else
        {
            if (brain.InRange(maxAttackRange))
                count += Time.deltaTime;
            else
                count -= Time.deltaTime / 2;
            count = Mathf.Clamp(count, 0, count);
        }
        if(brain.state == EnemyStates.ATTACKING || brain.state == EnemyStates.ENRAGED)
        {
            animationTimer += Time.deltaTime;
            if(animationTimer >= currentWaitingTime)
            {
                windUp = false;
                animationTimer = float.MinValue;
                brain.an.SetTrigger("NextState");
            }
        }
        
        if(windUp)
        {
            //Debug.Log("Look at player");
            brain.LookAtPlayer();
        }
    }

    //Set the universal trigger for animations to go to the next stage
    public void SetTrigger()
    {
        brain.an.SetTrigger("NextState");
    }

    //Set the windup time for the animation and begins the windup
    public void WindUpTrigger(int attack)
    {
        
        brain.an.SetFloat("AttackMod", 1);
        attack = attack - 1;
        if (attack < 0 && attack >= attackWindUpSeconds.Length)
        {
            Debug.LogError("Attack value for WindUp invalid");
            return;
        }
        if (attackWindUpSeconds[attack] == 0)
        {
            
            animationTimer = float.MinValue;
            windUp = true;
            return;
        }
        currentWaitingTime = attackWindUpSeconds[attack];
        if(animationTimer<0 && brain.an.GetBool("Attacking"))
            animationTimer = 0;
        windUp = true;
    }

    //Sets the wind down time for the animation and begins the winddown
    public void WindDownTrigger(int attack)
    {
        brain.an.SetFloat("AttackMod", 1);
        attack = attack - 1;
        if (attack < 0 && attack >= attackWindDownSeconds.Length)
        {
            Debug.LogError("Attack value for WindDown invalid");
            return;
        }
        currentWaitingTime = attackWindDownSeconds[attack];
        if (animationTimer < 0 && brain.an.GetBool("Attacking"))
            animationTimer = 0;
    }

    //triggers the attack and sets the speed of the animation to match
    public void AttackTrigger(int attack)
    {
        attack = attack - 1;
        if (attack < 0 && attack >= attackSeconds.Length)
        {
            Debug.LogError("Attack value for SetAttackSpeed invalid");
            return;
        }
        brain.an.SetFloat("AttackMod", (float)attackFrames[attack] / (attackFramesPerSecond * attackSeconds[attack]));
    }

    //when needed, the animation will skip ahead to keep consistent with the world events
    public void ForceAnimationChange()
    {
        animationTimer = float.MaxValue;
    }

    //attack has ended and resets enemy states to default
    public virtual void AttackEnd()
    {
        count = 0;
        if(brain.state != EnemyStates.DEAD && brain.state != EnemyStates.ENRAGED)
            brain.state = EnemyStates.NOTHING;
        windUp = false;
        brain.an.SetBool("Attacking", false);
        animationTimer = float.MinValue;
        //Debug.Log(Time.realtimeSinceStartup - timeTest);
    }

}
