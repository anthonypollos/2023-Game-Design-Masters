using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttackTemplate : MonoBehaviour
{
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
    public abstract void Attack();

    protected float count = 0;

    protected void UpdateCounter()
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
        
    }
}
