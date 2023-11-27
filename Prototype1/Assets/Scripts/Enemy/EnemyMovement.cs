using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public bool isMoving = true;
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

    private Vector3 previousPosition;
    private float refreshTime;
    private Vector3 targetPosition;
    private NavMeshPath path;
    private float count;
    private int corner;
    [HideInInspector]
    public Rigidbody rb;

    [SerializeField]
    [Tooltip("What range does the enemy want to be")]
    float optimalRange;

    [SerializeField]
    [Tooltip("How close player has to be for the enemy to try and retreat")]
    float tooCloseRange;
    // Start is called before the first frame update
    void Start()
    {
        previousPosition = transform.position;
        refreshTime = 0;
        rb = GetComponent<Rigidbody>();
        path = new NavMeshPath();
        targetPosition = transform.position;
        isforward = true;
        currentPoint = 0;
        corner = 0;
        if (isMoving)
        {
            RandomPoint(out targetPosition);
            if (isPatrolling)
                targetPosition = patrolPoints[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        count += Time.deltaTime;
        refreshTime += Time.deltaTime;
        if(!brain.isAggro && !isPatrolling)
        {
            if(refreshTime > 2f)
            {
                refreshTime = 0;
                if (Vector3.Distance(previousPosition, transform.position) < 0.5f)
                {
                    //Debug.Log("Stuck, resetting");
                    RandomPoint(out targetPosition);
                }
                previousPosition = transform.position;
            }
        }
    }

    public void Move()
    {

        if (isMoving)
        {
            if (count >= 1f || corner >= path.corners.Length)
            {
                if (!NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path))
                {
                    RandomPoint(out targetPosition);
                }
                else
                {
                    if (path.status != NavMeshPathStatus.PathComplete)
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
    }

    public void Stop()
    {
        if (isMoving)
        rb.velocity = Vector3.zero + rb.velocity.y * Vector3.up;
    }

    private void Movement(Vector3 dir)
    {
        dir.y = 0;
        rb.velocity = movementSpeed * (dir) + Vector3.up * rb.velocity.y;
        if (isMoving && brain.an!=null)
        {
            if (rb.velocity == Vector3.zero) brain.an.SetFloat("MoveState", 0);
            else if (!brain.isAggro) brain.an.SetFloat("MoveState", 1);
            else brain.an.SetFloat("MoveState", 2);
        }
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

            if (Vector3.Distance(targetPosition, transform.position) < 1f)
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
        if (isPatrolling) centerPoint = transform.position;
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
