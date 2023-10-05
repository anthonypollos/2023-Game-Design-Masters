using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] 
    float movementSpeed;
    [SerializeField] [Tooltip("Is the enemy a patrol type enemy or a random wandering enemy")] 
    bool isPatrolling;
    [Header("Patrolling variables")]
    [SerializeField]
    Vector3[] patrolPoints;
    int currentPoint;
    bool isforward;
    [Header("Wandering varaibles")]
    [SerializeField]
    Vector3 centerPoint;
    [SerializeField]
    float wanderRadius;
    [HideInInspector]
    public EnemyBrain brain;

    private Vector3 targetPosition;
    private NavMeshPath path;
    private float count;
    private int corner;
    [HideInInspector]
    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        path = new NavMeshPath();
        targetPosition = transform.position;
        isforward = true;
        currentPoint = 0;
        corner = 0;
        RandomPoint(out targetPosition);
        //get first path
        while(!NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path))
        {
            RandomPoint(out targetPosition);
        }
    }

    // Update is called once per frame
    void Update()
    {
        count += Time.deltaTime;
    }

    public void Move()
    {
        
        if (count>=1f || corner>=path.corners.Length)
        {
            if(!NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path))
            {
                RandomPoint(out targetPosition);
            }
            else
            {
                if(path.status != NavMeshPathStatus.PathComplete)
                {
                    RandomPoint(out targetPosition);
                }
            }
        }

        if (corner < path.corners.Length && Vector3.Distance(transform.position, path.corners[corner]) < 1.0f)
        {
            corner++;
        }

        MovementCalc();
    }

    public void Stop()
    {
        rb.velocity = Vector3.zero + rb.velocity.y * Vector3.up;
    }

    private void Movement(Vector3 dir)
    {
        dir.y = 0;
        rb.velocity = movementSpeed * (dir) + Vector3.up * rb.velocity.y;

    }

    private void MovementCalc()
    {
        Vector3 dir;
        //Debug.Log(path.corners[corner]);
        if (corner < path.corners.Length)
        {
            dir = (path.corners[corner] - transform.position).normalized;
            //Debug.Log(path.corners[corner]);
        }
        else
        {
            dir = (targetPosition - transform.position).normalized;
        }
        Movement(dir);
        if (brain.isAggro)
        {
            //Debug.Log("hunt");
            targetPosition = brain.player.position;
            //Debug.Log(Vector3.Distance(player.position, transform.position) + " + " + CanSee());
            
        }
        else
        {

            if (Vector3.Distance(targetPosition, transform.position) < 2f)
            {
                if (!isPatrolling)
                {
                    Vector3 target;
                    //Debug.Log("Try random point");
                    if (RandomPoint(out target))
                    {
                        //Debug.Log("Random Point");
                        targetPosition = target;
                    }
                }
                else
                {
                    if (isforward)
                    {
                        currentPoint++;
                        if(currentPoint >= patrolPoints.Length)
                        {
                            currentPoint = patrolPoints.Length - 2;
                            targetPosition = patrolPoints[currentPoint];
                            isforward = false;
                        }
                    }
                    else
                    {
                        currentPoint--;
                        if(currentPoint<0)
                        {
                            currentPoint = 1;
                            targetPosition = patrolPoints[currentPoint];
                            isforward = true;
                        }
                    }
                }

            }
        }

    }

    private bool RandomPoint(out Vector3 output)
    {
        Vector3 randomPoint = centerPoint + Random.insideUnitSphere * wanderRadius;
        NavMeshHit hit;
        for (int i = 0; i < 100; i++)
        {
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                output = hit.position;
                return true;
            }
        }
        //Debug.Log("fail");
        output = Vector3.zero;
        return false;
    }
}
