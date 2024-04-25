using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using FMOD;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyMovement : MonoBehaviour, ISlowable
{
    public bool isMoving = true;
    [SerializeField] bool isIdle = false;
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
    public float wanderRadius;
    [HideInInspector]
    public EnemyBrain brain;

    private Vector3 previousPosition;
    private float refreshTime;
    private Vector3 targetPosition;
    private NavMeshPath path;
    //private NavMeshPath previousPath;
    private List<Vector3> corners;
    private List<Vector3> prevCorners;
    private float count;
    private int corner;
    [HideInInspector]
    public Rigidbody rb;



    [SerializeField]
    [Tooltip("How close player has to be for the enemy to try and retreat")]
    float tooCloseRange;

    [SerializeField]
    [Tooltip("Radius possible that the enemy will think the player is at when it comes to navigation")]
    float offsetRadius;
    [SerializeField]
    [Tooltip("Read Only")]
    Vector2 offset = Vector2.zero;



    int lastValue;

    List<float> slowMods;
    float[] slowModsArray;

    [SerializeField] bool debug = false;

    // Start is called before the first frame update
    void Start()
    {
        EnterSlowArea(0);
        //If we don't change the center point from 0 0 0, assume we want the center point to be the spawn location.
        if (centerPoint == Vector3.zero) centerPoint = transform.position;
        previousPosition = transform.position;
        refreshTime = 0;
        rb = GetComponent<Rigidbody>();
        path = new NavMeshPath();
        corners = new List<Vector3>();
        prevCorners = new List<Vector3>();
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
        offset = Random.insideUnitCircle * offsetRadius;
    }

    // Update is called once per frame
    void Update()
    {
        count += Time.deltaTime;
        if (brain.state == EnemyStates.ATTACKING || brain.state == EnemyStates.DEAD)
        {
            refreshTime = 0;
        }
        else
        {
            refreshTime += Time.deltaTime;
        }
        if (!brain.isAggro && !isPatrolling)
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

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, .125f);
        Gizmos.DrawWireSphere(transform.position, tooCloseRange);
    }

    public void Move()
    {

        if (isMoving && brain.state != EnemyStates.DEAD)
        {
            if (count >= 1f || corner >= corners.Count)
            {
                isIdle = false;
                SavePrevious();
                if (!NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path))
                {
                    if (debug)
                        UnityEngine.Debug.Log("Path failed");
                    if(!brain.isAggro)
                        RandomPoint(out targetPosition);
                    else
                    {
                        if (corner < corners.Count)
                        {
                            LoadPrevious();
                        }
                        else
                        {
                            Stop();
                        }
                    }
                }
                else
                {
                    
                    if (debug)
                    {
                        Vector3 previousPoint = transform.position;
                        foreach (Vector3 point in corners)
                        {
                            UnityEngine.Debug.DrawLine(previousPoint, point, Color.magenta);
                            previousPoint = point;
                        }
                    }
                    if(debug) 
                        UnityEngine.Debug.Log("Path written");
                    
                    if (path.status != NavMeshPathStatus.PathComplete)
                    {
                        if (debug)
                            UnityEngine.Debug.Log("Path incomplete");
                        if(!brain.isAggro)
                            RandomPoint(out targetPosition);
                        else
                        {
                            if(corner < corners.Count) 
                            { 
                                LoadPrevious(); 
                            }
                            else
                            {
                                Stop();
                            }
                                
                        }
                    }
                    else
                    {
                        LoadPath();
                        corner = 0;
                    }
                }
            }

            if (corner < corners.Count && Vector3.Distance(transform.position, corners[corner]) < 1.0f)
            {
                corner++;
            }
            MovementCalc();
        }
    }

    private void SavePrevious()
    {
        prevCorners.Clear();
        prevCorners.AddRange(corners);
    }

    private void LoadPrevious()
    {
        corners.Clear();
        corners.AddRange(prevCorners);
    }

    private void LoadPath()
    {
        corners.Clear();
        corners.AddRange(path.corners);
    }

    public void Stop()
    {
        if (isMoving)
        {
            isIdle = true;
            brain.an.SetFloat("MoveState", 0);
            rb.velocity = Vector3.zero + rb.velocity.y * Vector3.up;
            if(path.status == NavMeshPathStatus.PathInvalid)
            {
                MoveBackIntoZone();
            }
        }
    }

    private void Movement(Vector3 dir)
    {
        dir.y = 0;
        dir = dir.normalized;
        if (debug)
        {
            UnityEngine.Debug.Log(dir);
            UnityEngine.Debug.DrawLine(rb.position, rb.position + dir * 6, Color.red);
        }
        float adjustedMS = movementSpeed * (1 - Mathf.Max(slowModsArray));
        rb.velocity = adjustedMS * (dir) + Vector3.up * rb.velocity.y;
        if (isMoving && brain.an!=null)
        {
            if (dir == Vector3.zero) brain.an.SetFloat("MoveState", 0);
            else if (!brain.isAggro) brain.an.SetFloat("MoveState", 1);
            else brain.an.SetFloat("MoveState", 2);
        }
    }

    private void MoveBackIntoZone()
    {
        Vector3 dir = Vector3.zero;
        if (brain.CheckLOS(transform.position, brain.player))
        {
            dir = (brain.player.position - transform.position);
            dir.y = 0;
            dir = dir.normalized;
        }
        else
        { 
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
            {
                dir = (hit.position - transform.position);
                dir.y = 0;
                dir = dir.normalized;
            }
            else
            {
                UnityEngine.Debug.Log("Can't return");
            }

        }
        float adjustedMS = movementSpeed * (1 - Mathf.Max(slowModsArray));
        rb.velocity = adjustedMS * (dir) + Vector3.up * rb.velocity.y;
        if (isMoving && brain.an != null)
        {
            if (dir == Vector3.zero) brain.an.SetFloat("MoveState", 0);
            else if (!brain.isAggro) brain.an.SetFloat("MoveState", 1);
            else brain.an.SetFloat("MoveState", 2);
        }
    }

    public void EnterSlowArea(float slowPercent)
    {
        if (slowMods == null)
        {
            slowMods = new List<float>();
        }
        slowMods.Add(slowPercent);
        slowModsArray = slowMods.ToArray();
    }
    public void ExitSlowArea(float slowPercent)
    {
        if (slowMods != null)
        {
            if (slowMods.Contains(slowPercent))
                slowMods.Remove(slowPercent);
            slowModsArray = slowMods.ToArray();
        }
    }

    private void MovementCalc()
    {
        if (!isIdle)
        {
            Vector3 dir;
            //Debug.Log(path.corners[corner]);
            if (corner < corners.Count)
            {
                dir = (corners[corner] - transform.position).normalized;
                //Debug.Log(path.corners[corner]);
            }
            else
            {
                dir = (targetPosition - transform.position).normalized;
            }
            Movement(dir);
        }
        if (brain.isAggro)
        {
            CalculateAggroMovement();

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

    private void CalculateAggroMovement()
    {

        float distance = Vector3.Distance(brain.player.position, transform.position);
        if (distance <= brain.optimalRange && brain.CanSee(brain.player))
        {
            Vector3 dir = (brain.player.position - transform.position).normalized;
            //if too close
            if (distance < tooCloseRange)
            {
                //Debug.Log("too close");
                dir *= -1;
            }
            //if at optimal range
            else
            {
                float angle =
                Mathf.Lerp(90f, 50f, (distance - tooCloseRange) / (brain.optimalRange - tooCloseRange));
                //Debug.Log("Just Right");
                if (refreshTime >= 1f)
                {
                    lastValue = Random.Range(0, 2);
                    refreshTime = 0;
                }
                switch (lastValue)
                {
                    case 0:
                        dir = Quaternion.Euler(0, angle, 0) * dir;
                        break;
                    case 1:
                        dir = Quaternion.Euler(0, -angle, 0) * dir;
                        break;
                    default:
                        dir = Vector3.zero;
                        break;
                }
            }

            targetPosition = transform.position + dir * movementSpeed;
        }

        else
        {
            Vector3 temp = Vector3.zero;
            temp.x = brain.player.position.x + offset.x;
            temp.y = brain.player.position.y;
            temp.z = brain.player.position.z + offset.y;
            if (brain.CheckLOS(temp, brain.player))
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(temp, out hit, 2.0f, NavMesh.AllAreas))
                {
                    temp = hit.position;
                    targetPosition = temp;
                }
                else
                {
                    targetPosition = brain.player.position;
                }
                
            }
            else
            {
                targetPosition = brain.player.position;
            }
        }
        SavePrevious();
        if (!NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path))
        {
            LoadPrevious();
        }
        else
        {
            if (path.status != NavMeshPathStatus.PathComplete)
            {
                
                if (corner < corners.Count)
                {
                    LoadPrevious();
                }
                else
                {
                    Stop();
                }

                
            }
            else
            {
                LoadPath();
                corner = 0;
            }
        }

       
    }

    private bool RandomPoint(out Vector3 output)
    {
        if (isPatrolling) centerPoint = transform.position;
        Vector3 randomPoint = centerPoint + Random.insideUnitSphere * wanderRadius;
        randomPoint.y = centerPoint.y;
        NavMeshHit hit;
        for (int i = 0; i < 100; i++)
        {
            if (NavMesh.SamplePosition(randomPoint, out hit, 2.0f, NavMesh.AllAreas))
            {
                output = hit.position;
                return true;
            }
        }
        if(debug)
            UnityEngine.Debug.Log("fail");
        output = Vector3.zero;
        return false;
    }
}

//Visualizer for changing the Enemy's wander radius.
//Tutorial followed: https://www.youtube.com/watch?v=ABuXRbJDdXs
//Sean did this. If this screws things up, blame me.
#if UNITY_EDITOR
[CustomEditor(typeof(EnemyMovement))]
public class EnemyMovementEditor : Editor
{
    public void OnSceneGUI()
    {
        //Link the EnemyMovement script into this editor class
        var linkedObject = target as EnemyMovement;

        //set the handle colors.
        Handles.color = Color.green;

        //begin a check to see if we've changed anything.
        EditorGUI.BeginChangeCheck();

        //create a new float based on where we've dragged the radius sphere
        float newWanderRadius = Handles.RadiusHandle(Quaternion.identity, linkedObject.transform.position, linkedObject.wanderRadius, false);
       
        //check to see if the range has been changed
        if (EditorGUI.EndChangeCheck())
        {
            //if the range has been changed, we record that.
            Undo.RecordObject(target, "Update Range");
            //Now, we replace our wander radius with the new wander radius made by dragging the wander radius sphere. Yippeeeeee!!!!!!
            linkedObject.wanderRadius = newWanderRadius;
        }
    }
}
#endif