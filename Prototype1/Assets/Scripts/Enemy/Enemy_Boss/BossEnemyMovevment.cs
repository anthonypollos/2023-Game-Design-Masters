using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyMovevment : EnemyMovement
{
    [HideInInspector]
    public BossEnemyBrain bossBrain;
    [SerializeField]
    float chargeMaxSpeed = 10;
    [SerializeField]
    [Tooltip("Higher = turns sharper")]
    float chargeTurningMomentum;

    protected override void Starting()
    {
        base.Starting();
        offset = Vector2.zero;
    }

    protected override void Movement(Vector3 dir)
    {
        if (brain.state == EnemyStates.CHARGING)
        {
            bossBrain.an.SetFloat("MoveState", 3);

            dir.y = 0;
            dir = dir.normalized;
            if (debug)
            {
                UnityEngine.Debug.Log(dir);
                UnityEngine.Debug.DrawLine(rb.position, rb.position + dir * 6, Color.red);
            }
            rb.AddForce(dir * rb.mass * chargeTurningMomentum, ForceMode.Impulse);
            float adjustedMS = chargeMaxSpeed * (1 - Mathf.Max(slowModsArray));
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Min(rb.velocity.y, 0), rb.velocity.z);
            rb.velocity = rb.velocity.normalized * adjustedMS;
            Vector3 velocityNoY = rb.velocity - new Vector3(0, rb.velocity.y);
            if(velocityNoY!=Vector3.zero)
                transform.forward = velocityNoY.normalized;
            //insert charging movestate animation tag here
        }
        else
        {
            base.Movement(dir);
        }
    }
}
