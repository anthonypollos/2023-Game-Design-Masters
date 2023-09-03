using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] Material[] materials;
    //Material debugStartingMaterial;
    MeshRenderer mr;
    bool moved;
    private bool stunned;

    //AI required code
    private Transform player;
    //private NavMeshAgent agent;
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
    private bool lockedOn;
    private bool canShoot;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
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
    }

    private void Update()
    {
        if (!stunned)
        {
            //Debug.Log("move");
            Movement();

            if (!lockedOn)
            {
                if (Vector3.Distance(player.position, transform.position) < aggroDistance)
                {
                    Aggro();
                }
            }
        }
        if (transform.position.y < -5f) Destroy(gameObject);
    }
    private void Movement()
    {

        Vector3 dir = (targetPosition - transform.position).normalized;
        //if y component over a certain amount Jump
        dir.y = 0;
        rb.velocity = 3 * (dir) + new Vector3(0, rb.velocity.y, 0);
        if (lockedOn)
        {
            //Debug.Log("hunt");
            transform.LookAt(player);
            targetPosition = player.position;
            if(Vector3.Distance(player.position, transform.position)<enemyDistance)
            {
                rb.velocity = Vector3.zero;
            }
            if (canShoot && Vector3.Distance(player.position, transform.position) < (enemyDistance + rangeBuffer))
            {
                StartCoroutine(Shoot());
            }
        }
        else
        {
            
            if (Vector3.Distance(targetPosition, transform.position) < 1f)
            {
                Vector3 target;
                //Debug.Log("Try random point");
                if(RandomPoint(out target))
                {
                    Debug.Log("Random Point");
                    targetPosition = target;
                    transform.LookAt(target);
                }

            }
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

    private IEnumerator Shoot()
    {
        canShoot = false;
        yield return new WaitForSeconds(projectileCD);
        Vector3 dir = (player.transform.position - transform.position).normalized;
        Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>().velocity = (dir * projectileSpeed);
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
        //insert unstunned code here
    }
    private IEnumerator Moved()
    {
        Stunned();
        yield return new WaitForSeconds(0.1f);
        moved = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Moved: " + moved + "\ncollision tag: " + collision.gameObject.tag);
        if(moved && collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Unstun");
            moved = false;
            UnStunned();
        }
    }



    
}
