using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttackTemplate : MonoBehaviour
{
    [Header("Starting attack data")]
    [SerializeField] 
    [Tooltip("Range at which the enemy can start attacking")]
    public float maxAttackRange;
    [SerializeField]
    [Tooltip("Range at which the enemy will always do an attack")]
    protected float minAttackRange;
    [SerializeField]
    [Tooltip("Time it takes once in range to attack")]
    public float attackSpeed;
    [HideInInspector]
    public EnemyBrain brain;
    [Header("Attack timing information")]
    [SerializeField] protected float[] attackWindUpSeconds;
    [SerializeField] protected float[] attackSeconds;
    [SerializeField] protected float[] attackWindDownSeconds;
    [Header("Attack frame data")]
    [SerializeField] protected int[] attackFrames;
    [SerializeField] protected int[] windUpFrames;
    [SerializeField] protected int[] windDownFrames;
    [SerializeField] protected int attackFramesPerSecond;
    

    protected float timeTest;
    public abstract void Attack();

    protected float count = 0;

    protected float animationTimer = float.MinValue;

    protected float currentWaitingTime = float.MaxValue;

    protected virtual void UpdateCounter()
    {
        if (brain.interaction.stunned)
            count = 0;
        else
        {
            if (brain.InRange(maxAttackRange))
                count += Time.deltaTime;
            else
                count -= Time.deltaTime / 2;
            count = Mathf.Clamp(count, 0, attackSpeed);
        }
        if(brain.state == EnemyStates.ATTACKING )
        {
            animationTimer += Time.deltaTime;
            if(animationTimer >= currentWaitingTime)
            {
                animationTimer = float.MinValue;
                //ToDo add animation stuff here
            }
        }
        
    }

    public void WindUpTrigger(int attack)
    {
        brain.an.SetFloat("AttackMod", 1);
        attack = attack - 1;
        if (attack < 0 && attack >= attackWindUpSeconds.Length)
        {
            Debug.LogError("Attack value for WindUp invalid");
            return;
        }
        currentWaitingTime = attackWindUpSeconds[attack];
        if(animationTimer<0)
            animationTimer = 0;
    }

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
        if(animationTimer<0)
            animationTimer = 0;
    }

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



    public void SetWindUpSpeed(int attack)
    {
        attack = attack - 1;
        if (attack<0 && attack>=attackWindUpSeconds.Length)
        {
            Debug.LogError("Attack value for SetWindUpSpeed invalid");
            return;
        }
        brain.an.SetFloat("AttackMod", (float)windUpFrames[attack] / (attackFramesPerSecond * attackWindUpSeconds[attack]));
    }

    public void SetAttackSpeed(int attack)
    {
        attack = attack - 1;
        if (attack < 0 && attack >= attackSeconds.Length)
        {
            Debug.LogError("Attack value for SetAttackSpeed invalid");
            return;
        }
        brain.an.SetFloat("AttackMod", (float)attackFrames[attack] / (attackFramesPerSecond * attackSeconds[attack]));
    }

    public void SetWindDownSpeed(int attack)
    {
        attack = attack - 1;
        if (attack < 0 && attack >= attackWindDownSeconds.Length)
        {
            Debug.LogError("Attack value for SetWindDownSpeed invalid");
            return;
        }
        brain.an.SetFloat("AttackMod", (float)windDownFrames[attack] / (attackFramesPerSecond * attackWindDownSeconds[attack]));
    }
}
