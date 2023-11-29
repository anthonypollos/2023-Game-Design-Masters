using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyStates
{
    public const int NOTHING = 0;
    public const int ATTACKING = 1;
    public const int DEAD = 2;
}

public class EnemyBrain : MonoBehaviour, IEnemy
{
    [HideInInspector]
    public EnemyHealth health;
    [HideInInspector]
    public EnemyInteractionBehaviorTemplate interaction;
    EnemyMovement movement;
    EnemyAttackTemplate attack;
    [HideInInspector]
    public Moveable moveable;
    [HideInInspector]
    public Transform player;
    [SerializeField] 
    [Tooltip("Range in which the enemy can see the player or any other enemy getting aggroed")]
    float sightDistance;
    [HideInInspector]
    public bool isAggro;
    [HideInInspector]
    public Animator an;
    [SerializeField] 
    [Tooltip("What layers block LOS from the player")]
    LayerMask layermask;
    [HideInInspector]
    public int state;
    [SerializeField] 
    [Tooltip("What distance does the creature want to stay in from the player")]
    public float optimalRange;
    

    // Start is called before the first frame update
    void Start()
    {
        player = GameController.GetPlayer();
        //Debug.Log(player);
        isAggro = false;
        moveable = GetComponent<Moveable>();
        an = GetComponent<Animator>();
        an.logWarnings = false;
        health = GetComponent<EnemyHealth>();
        health.brain = this;
        interaction = GetComponent<EnemyInteractionBehaviorTemplate>();
        interaction.brain = this;
        movement = GetComponent<EnemyMovement>();
        movement.brain = this;
        attack = GetComponent<EnemyAttackTemplate>();
        if(attack !=null)
            attack.brain = this;
        
        state = EnemyStates.NOTHING;
    }

    // Update is called once per frame
    void Update()
    {
        if (state != EnemyStates.DEAD)
        {
            //Debug.Log(state);
            if (!interaction.stunned && state == EnemyStates.NOTHING)
            {
                CheckMovement();
                CheckRotation();
                CheckAttack();
                CheckArea();
            }
            else if (interaction.stunned && moveable != null)
            {
                if (!moveable.isLaunched)

                    movement.Stop();
            }
        }
    }

    void CheckMovement()
    {
        if (InRange(optimalRange))
        {
            movement.Move();
        }
        else
        {
            movement.Move();
        }
    }

    void CheckRotation()
    {
        if(state == EnemyStates.NOTHING)
        {
            if (movement.rb.velocity.x != 0 || movement.rb.velocity.z != 0)
                if(movement.isMoving)
                    transform.forward = movement.rb.velocity.normalized;
        }
    }

    void CheckArea()
    {
        if (CanSeePlayer())
            PackAggro();
    }

    public void LookAtPlayer()
    {
        Vector3 dir = (player.transform.position - transform.position).normalized;
        dir.y = 0;
        transform.forward = dir.normalized;
    }

    void CheckAttack()
    {
        if(attack!=null)
            if (InRange(attack.maxAttackRange) && isAggro)
                attack.Attack();
    }

    public bool InRange(float distance)
    {
        return (Vector3.Distance(player.position, transform.position) < distance && CanSeePlayer());
    }


    private bool CanSeePlayer()
    {
        if (Vector3.Distance(player.position, transform.position) > sightDistance) return false;
        RaycastHit hit;
        //Debug.DrawRay(transform.position, player.position - transform.position, Color.red);
        if (Physics.Raycast(transform.position, player.position - transform.position, out hit, Mathf.Infinity, layermask))
        {
            //Debug.Log(hit.transform.name);
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                return true;
            }
            else
            {
                //Debug.Log(hit.transform.name);
                return false;
            }
        }
        //Debug.Log("no hit");
        return false;
    }

    public bool CanSee(Transform target)
    {
        if (Vector3.Distance(target.position, transform.position) > sightDistance) return false;
        RaycastHit hit;
        //Debug.DrawRay(transform.position, player.position - transform.position, Color.red);
        if (Physics.Raycast(transform.position, target.position - transform.position, out hit, Mathf.Infinity, layermask))
        {
            //Debug.Log(hit.transform.name);
            if (hit.transform == target)
            {
                return true;
            }
            else
            {
                //Debug.Log(hit.transform.name);
                return false;
            }
        }
        //Debug.Log("no hit");
        return false;
    }

    public void PackAggro()
    {
        Aggro();
        foreach(EnemyBrain enemy in FindObjectsOfType<EnemyBrain>())
        {
            if(enemy.CanSee(transform))
            {
                enemy.Aggro();
            }
        }
    }
    public void Aggro()
    {
        if (!isAggro)
        {
            isAggro = true;
            health.ec.AddAggro(gameObject);
        }
        
    }

    public void Deaggro()
    {
        throw new System.NotImplementedException();
    }



}
