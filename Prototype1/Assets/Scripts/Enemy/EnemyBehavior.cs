using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour, IPullable, IKickable, IDamageable
{
    [SerializeField] Material[] materials;
    //Material debugStartingMaterial;
    MeshRenderer mr;
    

    //AI required code
    private Transform player;
    #region movementVariables
    [SerializeField] float enemyDistance;
    [SerializeField] float aggroDistance;
    [SerializeField] float rangeBuffer = 2;
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed;
    [SerializeField] float projectileCD;
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
    private bool lockedOn;
    private bool canShoot;
    private Rigidbody rb;
    bool moved;
    private bool stunned;
    [SerializeField] LayerMask layerMask;
    #endregion combatVariables
    // Start is called before the first frame update
    void Start()
    {
        count = 1;
        corner = 0;
        mr = GetComponent<MeshRenderer>();
        moved = false;
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
                //Debug.Log("Calculate Path failed");
            }
        }
        if (!stunned)
        {
            //Debug.Log("move");
            MovementCalc();

            if (!lockedOn)
            {
                if (Vector3.Distance(player.position, transform.position) < aggroDistance)
                {
                    Aggro();
                }
            }
        }
        if(corner<path.corners.Length && Vector3.Distance(transform.position, path.corners[corner])<1.0f)
        {
            corner++;
        }
        if (transform.position.y < -5f) Destroy(gameObject);
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
            if(Vector3.Distance(player.position, transform.position)<enemyDistance && CanSee())
            {
                transform.LookAt(player.position);
                rb.velocity = Vector3.zero;
                inRange = true;
                
            }
            else
            {
                inRange = false;
            }
            if (canShoot && Vector3.Distance(player.position, transform.position) < (enemyDistance + rangeBuffer) && CanSee())
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
            rb.velocity = speed * (dir) + new Vector3(0, rb.velocity.y, 0);
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

    private bool CanSee()
    {
        RaycastHit hit;
        //Debug.DrawRay(transform.position, player.position - transform.position, Color.red);
        if(Physics.Raycast(transform.position, player.position-transform.position, out hit, Mathf.Infinity, layerMask))
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
    private void Aggro()
    {
        foreach (EnemyBehavior enemy in GameObject.FindObjectsByType<EnemyBehavior>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            //insert code to have a distance modifier if wanted (enemies would be "inactive" if in another room)
            enemy.PackAggro();
        }
    }

    public void PackAggro()
    {
        lockedOn = true;
        mr.material = materials[0];
    }

    public void Lassoed()
    {
        Stunned();
        //Insert lassoed animation
    }

    public void Pulled()
    {
        if(!moved)
            StartCoroutine(Moved());
        //Insert pulled animation
    }

    public void Kicked()
    {
        if (!moved)
            StartCoroutine(Moved());
        //Insert damage here
        //Insert kicked animation here
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
    private IEnumerator Moved()
    {
        Stunned();
        yield return new WaitForSeconds(0.1f);
        moved = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        //Debug.Log("Moved: " + moved + "\ncollision tag: " + collision.gameObject.tag);
        if(moved && collision.gameObject.CompareTag("Ground"))
        {
            //Debug.Log("Unstun");
            moved = false;
            UnStunned();
        }
    }

    public void TakeDamage(int dmg)
    {

    }

    #endregion combat



}
