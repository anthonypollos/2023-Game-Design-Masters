using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour, IPullable, IKickable, IEnemy
{
    [SerializeField] Material[] materials;
    //Material debugStartingMaterial;
    MeshRenderer mr;
    

    //AI required code
    private Transform player;
    private EnemyContainer ec;
    private Moveable moveable;
    #region movementVariables
    [SerializeField] float enemyDistance;
    [SerializeField] float aggroDistance;
    [SerializeField] float rangeBuffer = 2;
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed;
    [SerializeField] [Tooltip("projectile cooldown")] float projectileCD;
    [SerializeField] [Tooltip("Center of the patrol circle")] Vector3 centerPoint;
    [SerializeField] float patrolRadius;
    [SerializeField] float speed = 3f;
    private Vector3 targetPosition;
    private Vector3 moveTowards;
    private NavMeshPath path;
    private float count;
    private int corner;
    private bool inRange;
    #endregion movementVariables
    #region combatVariables
    private EnemyHealth eh;
    //[SerializeField][Tooltip("Damage enemies take when hitting a wall")] int wallDamage = 15;
    [SerializeField][Tooltip("Damage enemies take when hitting each other")] int clashDamage = 15;
    private bool lockedOn;
    private bool canShoot;
    private Rigidbody rb;
    private bool stunned;
    private bool hasCollided;
    [SerializeField] LayerMask layerMask1;
    [SerializeField] LayerMask layerMask2;
    [SerializeField] float deaggroTime = 10f;
    float deaggroCurrentTime;
    bool launching;
    #endregion combatVariables

    // Start is called before the first frame update
    void Start()
    {
        launching = false;
        moveable = GetComponent<Moveable>();
        ec = FindObjectOfType<EnemyContainer>();
        //ec.AddEnemy(gameObject);
        deaggroCurrentTime = 0;
        hasCollided = false;
        eh = GetComponent<EnemyHealth>();
        count = 1;
        corner = 0;
        mr = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        mr.material = materials[2];
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //agent = GetComponent<NavMeshAgent>();
        targetPosition = transform.position;
        canShoot = true;
        lockedOn = false;
        stunned = false;
        moveTowards = transform.position;
        path = new NavMeshPath();
        inRange = false;
        RandomPoint(out targetPosition);
    }

    private void Update()
    {
        count += Time.deltaTime;
        if(count>=1f || corner>=path.corners.Length)
        {
            count = 0;
            if(!NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path))
            {

                //Debug.Log("calculate path failed");
                if(!lockedOn)
                    RandomPoint(out targetPosition);
                deaggroCurrentTime += Time.deltaTime;
            }
            else
            {
                if(path.status != NavMeshPathStatus.PathComplete)
                {
                    if(!lockedOn)
                        RandomPoint(out targetPosition);
                    deaggroCurrentTime += Time.deltaTime;

                }
                else
                {
                    if(lockedOn)
                    {
                        deaggroCurrentTime = 0;
                    }
                }
            }
            if(deaggroCurrentTime>=deaggroTime)
            {
                Deaggro();
            }
        }
        if (moveable.isLaunched)
        {
            launching = true;
        }
        if(launching && !moveable.isLaunched)
        {
            hasCollided = false;
            UnStunned();
            launching = false;
        }
        if (!stunned && !moveable.isLaunched)
        {
            //Debug.Log("move");
            MovementCalc();

            if (!lockedOn)
            {
                if (Vector3.Distance(player.position, transform.position) < aggroDistance)
                {
                    PackAggro();
                }
            }
        }
        if(corner<path.corners.Length && Vector3.Distance(transform.position, path.corners[corner])<1.0f)
        {
            corner++;
        }
        if (transform.position.y < -5f) eh.TakeDamage(999999999);
    }
    #region movement 
    private void MovementCalc()
    {
        Vector3 dir;
        //Debug.Log(path.corners[corner]);
        if (corner < path.corners.Length)
        {
            dir = (path.corners[corner] - transform.position).normalized;
            Vector3 lookDir = transform.position + new Vector3(dir.x, 0, dir.y);
            transform.LookAt(lookDir);
            //Debug.Log(path.corners[corner]);
        }
        else
        {
            dir = (targetPosition - transform.position).normalized;
            Vector3 lookDir = transform.position + new Vector3(dir.x, 0, dir.y);
            transform.LookAt(lookDir);
        }
        Movement(dir);
        if (lockedOn)
        {
            //Debug.Log("hunt");
            targetPosition = player.position;
            //Debug.Log(Vector3.Distance(player.position, transform.position) + " + " + CanSee());
            if(Vector3.Distance(player.position, transform.position)<enemyDistance && CanSeePlayer())
            {
                transform.LookAt(player.position);
                rb.velocity = Vector3.zero;
                inRange = true;
                
            }
            else
            {
                inRange = false;
            }
            if (canShoot && Vector3.Distance(player.position, transform.position) < (enemyDistance + rangeBuffer) && CanSeePlayer())
            {
                transform.LookAt(player.position);
                StartCoroutine(Shoot());
            }
        }
        else
        {
            
            if (Vector3.Distance(targetPosition, transform.position) < 2f)
            {
                Vector3 target;
                //Debug.Log("Try random point");
                if(RandomPoint(out target))
                {
                    //Debug.Log("Random Point");
                    targetPosition = target;
                    transform.LookAt(target);
                }

            }
        }

    }

    private void Movement(Vector3 dir)
    {
        if (!inRange)
        {
            dir.y = 0;
            rb.velocity = speed * (dir) + Vector3.up * rb.velocity.y;
        }

    }

    private bool RandomPoint(out Vector3 output)
    {
        Vector3 randomPoint = centerPoint + Random.insideUnitSphere * patrolRadius;
        NavMeshHit hit;
        for (int i = 0; i< 100; i++)
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

    private bool CanSeePlayer()
    {
        RaycastHit hit;
        //Debug.DrawRay(transform.position, player.position - transform.position, Color.red);
        if(Physics.Raycast(transform.position, player.position-transform.position, out hit, Mathf.Infinity, layerMask1))
        {
            //Debug.Log(hit.transform.name);
            if(hit.transform.gameObject.CompareTag("Player"))
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
    
    private bool CanSee(GameObject target)
    {
        RaycastHit hit;
        //Debug.DrawRay(transform.position, player.position - transform.position, Color.red);
        if (Physics.Raycast(transform.position, target.transform.position - transform.position, out hit, Mathf.Infinity, layerMask2))
        {
            //Debug.Log(hit.transform.name);
            if (hit.transform.gameObject == target)
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

    #endregion movement

    #region combat
    private IEnumerator Shoot()
    {
        canShoot = false;
        Vector3 dir = (player.transform.position - transform.position).normalized;
        Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>().velocity = (dir * projectileSpeed);
        yield return new WaitForSeconds(projectileCD);
        canShoot = true;

    }
    private void PackAggro()
    {
        foreach (EnemyBehavior enemy in GameObject.FindObjectsByType<EnemyBehavior>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            //insert code to have a distance modifier if wanted (enemies would be "inactive" if in another room)
            if(CanSee(enemy.gameObject))
                enemy.Aggro();
        }
        Aggro();
    }

    public void Aggro()
    {
        lockedOn = true;
        mr.material = materials[0];
        ec.AddAggro(gameObject);
    }

    public void Deaggro()
    {
        lockedOn = false;
        mr.material = materials[2];
        ec.RemoveAggro(gameObject);
    }

    public void Lassoed()
    {
        Stunned();
        //Insert lassoed animation
    }

    public void Pulled(IsoAttackManager player = null)
    {
        //Insert pulled animation
    }

    public void Kicked()
    {
        //Insert kicked animation here
    }

    public void Stagger()
    {
        StopCoroutine(Staggered());
        StartCoroutine(Staggered());
    }

    private IEnumerator Staggered()
    {
        Stunned();
        yield return new WaitForSeconds(0.5f);
        if(!moveable.isLaunched)
            UnStunned();

    }


    private void Stunned()
    {
        mr.material = materials[1];
        stunned = true;
        //Aggro();
        //insert stunned code here
    }
    private void UnStunned()
    {
        //Debug.Log("change material back");
        //mr.material = materials[0];
        stunned = false;
        if(!lockedOn)
        {
            Aggro();
        }
        else
        {
            mr.material = materials[0];
        }
        //insert unstunned code here
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(moveable.isLaunched && !collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Lasso") && !hasCollided)
        {
            hasCollided = true;
            GameObject hit = collision.gameObject;
            eh.TakeDamage(clashDamage);
                IDamageable temp = hit.GetComponent<IDamageable>();
                if (temp != null)
                {
                    temp.TakeDamage(clashDamage);
                }
                ITrap temp2 = hit.GetComponent<ITrap>();
                if(temp2 != null)
                {
                    temp2.ActivateTrap(gameObject);
                }
       }
   }

    public void Break()
    {
        throw new System.NotImplementedException();
    }
}


    #endregion combat



