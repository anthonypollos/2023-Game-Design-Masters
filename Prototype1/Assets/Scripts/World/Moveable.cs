using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.SceneTemplate;
using UnityEngine;

public class Moveable : MonoBehaviour
{

    [SerializeField] int clashDamage = 10;
    [SerializeField] float neutralSpeed = 15f;
    [SerializeField] float rateOfChange = 1f;

    Rigidbody rb;
    Vector3 targetLocation;
    [SerializeField ] float speed;
    [HideInInspector] public bool isLaunched;
    Coroutine stopping;
    float buffer;
    //[SerializeField] float timeToStop = 1f;
    [SerializeField] float slowdownPerSecond = 30f;
    [HideInInspector]
    public bool isDashing;
    LayerMask groundLayers;
    //[SerializeField] float groundCheckBuffer = 0.1f;
    float boundsY;
    float boundsX;
    float boundsZ;
    Collider col;
    bool isStopping = false;
    bool hold = false;
    //bool isThrowing = false;
    [HideInInspector]
    public IsoAttackManager tendrilOwner;
    Vector3 dir;
    bool unstoppable = false;
    List<Collider> collidersHit;
    Collider myCollider;
    Collider playerCollider;
    IDamageable myDamageable;

    // Start is called before the first frame update
    void Start()
    {
        myDamageable = GetComponent<IDamageable>();
        myCollider = GetComponent<Collider>();
        playerCollider = GameController.GetPlayer().GetComponent<Collider>();
        collidersHit = new List<Collider>();
        hold = false;
        col = GetComponent<Collider>();
        boundsY = col.bounds.size.y / 2;
        boundsX = col.bounds.size.x / 2;
        boundsZ = col.bounds.size.z / 2;
        string[] temp = { "Ground", "Ground_Transparent" };
        groundLayers = LayerMask.GetMask(temp);
        stopping = null;
        rb = GetComponent<Rigidbody>();
        targetLocation = transform.position;
        speed = 0;
        isLaunched = false;
        buffer = 0;
        
       
    }

    private void Update()
    {
        if (isLaunched)
            buffer += Time.deltaTime;
        //IsGrounded();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isLaunched && stopping==null)
        {

            if (!hold)
            {
                rb.velocity = dir.normalized * speed + Vector3.up * Mathf.Clamp(rb.velocity.y, Mathf.NegativeInfinity, 1);
            }
            else
            {
                rb.velocity = Vector3.zero + Vector3.up * rb.velocity.y;
            }
            Vector3 positionIgnoreY = transform.position;
            positionIgnoreY.y = 0;
            Vector3 targetIgnoreY = targetLocation;
            if (tendrilOwner != null && InputChecker.instance.GetInputType() == InputType.KaM)
            {
                targetIgnoreY = tendrilOwner.lb.GetMousePosition().Item2;
            }
                targetIgnoreY.y = 0;
            if ((Vector3.Distance(positionIgnoreY, targetIgnoreY) < 0.5f) && tendrilOwner != null)
            {
                //Debug.Log("Too close");

                hold = true;
                //Debug.Log("Force release");
                //tendrilOwner.ForceRelease();
                //tendrilOwner = null;

                //isThrowing = false;
                /*if (isDashing)
                    stopping = StartCoroutine(Stop());
                else
                    stopping = StartCoroutine(Tumbling());
                */

            }
            else
            {
                hold = false;
            }

            if(!hold && (Vector3.Distance(positionIgnoreY, targetIgnoreY) < 0.5f))
            {
                if(tendrilOwner != null)
                {
                    tendrilOwner.ForceRelease();
                    tendrilOwner = null;
                }

                if (isDashing)
                    stopping = StartCoroutine(Stop());
                else
                    stopping = StartCoroutine(Tumbling());
            }
        }
    }

    public float GetMass()
    {
        return rb.mass;
    }

    private void OnCollisionStay(Collision collision)
    {
        if(!collision.transform.CompareTag("Ground") && isLaunched && buffer>.1f)
        {
            //Debug.Log("collide stay");
            if (tendrilOwner != null)
            {
                //Debug.Log("Force release");
                tendrilOwner.ForceRelease();
                tendrilOwner = null;
            }
            //Debug.Log(collision.gameObject.name);
            //Debug.Log("Hit object");
            if (!isStopping)
                stopping = StartCoroutine(Stop());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("Ground") && isLaunched && (!isDashing || unstoppable))
        {
            collidersHit.Add(collision.collider);
            Physics.IgnoreCollision(myCollider, collision.collider, true);

            //tendril lets go
            if (tendrilOwner != null)
            {
                //Debug.Log("Force release");
                tendrilOwner.ForceRelease();
                tendrilOwner = null;
            }
            Moveable moveable = collision.transform.GetComponent<Moveable>();
            //calculate clash damage
            if (!unstoppable)
            {
                if (moveable == null)
                    myDamageable.TakeDamage(CalculateClashDamage());
                else if (!moveable.AlreadyHit(myCollider))
                {
                    myDamageable.TakeDamage(CalculateClashDamage());
                }
            }
            IDamageable damageable = collision.transform.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(CalculateClashDamage());
            }
            if (moveable != null)
            {
                if (!moveable.AlreadyHit(myCollider))
                { 
                    //Launching
                    if(!unstoppable)
                    {
                        Vector3 dir = collision.transform.position - collision.contacts[0].point;
                        dir.y = 0;
                        dir = dir.normalized;
                        moveable.Slammed(dir, rb.mass * speed / 2, myCollider);
                        speed /= 2;
                        ForceRelease();
                    }
                    else
                    {
                        Vector3 dir = transform.forward;
                        dir.y = 0;
                        int coin = Random.Range(0, 2);
                        int mod;
                        if(coin == 0)
                        {
                            mod = -1;
                        }
                        else
                        {
                            mod = 1;
                        }
                        dir = Quaternion.Euler(0, mod * 45, 0) * dir;
                        moveable.Slammed(dir, 2* moveable.GetMass() * speed / 2, myCollider);
                    }
                    IKickable kickable = collision.transform.GetComponent<IKickable>();
                    if(kickable != null)
                    {
                        kickable.Kicked();
                    }
                    moveable.ForceReleaseDelayed();
                    
                }
            }
            //if wall, hard stop
            else if (!isStopping)
            {
                EnemyAttackTemplate at = transform.GetComponent<EnemyAttackTemplate>();
                if(at != null)
                {
                    at.ForceAnimationChange();
                }
                stopping = StartCoroutine(Stop());
            }
            
            //Check for Trap
            ITrap trap = collision.transform.GetComponent<ITrap>();
            if(trap != null)
            {
                trap.ActivateTrap(gameObject);
            }
        }
    }

    int CalculateClashDamage()
    {
        //Debug.Log("speed: " + speed);
        //Debug.Log("Damage: " + Mathf.RoundToInt(clashDamage + clashDamage * ((speed - neutralSpeed) / (neutralSpeed * rateOfChange))));
        return Mathf.RoundToInt(clashDamage + (speed - neutralSpeed) / (neutralSpeed * rateOfChange));
    }

    public bool AlreadyHit(Collider collider)
    {
        return collidersHit.Contains(collider);
    }

    private IEnumerator Tumbling()
    {
        //Vector3 startingVelocity = rb.velocity;
        Vector3 dir = new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized;
        //startingVelocity.y = 0;
        while (speed > 0)
        {
            yield return new WaitForSeconds(0.01f);
            speed = Mathf.Clamp(speed - (slowdownPerSecond * 0.01f), 0, speed);
            //Debug.Log(speed + "-" + slowPerSecond * 0.01f + "=" + (speed - (slowPerSecond * 0.01f)));
            //Debug.Log("Speed = " + speed);
            rb.velocity = dir * speed + Vector3.up * rb.velocity.y;
        }

        stopping = StartCoroutine (Stop());
    }

    private IEnumerator Stop()
    {
        speed = 0;
        isStopping = true;
        rb.velocity = Vector3.zero + Vector3.up * rb.velocity.y;
        yield return new WaitForSeconds(0.01f);
        //yield return new WaitUntil(IsGrounded);
        isLaunched = false;
        rb.velocity = Vector3.zero + Vector3.up * rb.velocity.y;
        stopping = null;
        isStopping = false;
        unstoppable = false;
        ResetCollisions();
    }

    void ResetCollisions()
    {
        foreach(Collider collider in collidersHit)
        {
            Physics.IgnoreCollision(myCollider, collider, false);
        }
        collidersHit.Clear();
    }

    public void Launched(Vector3 target, float force)
    {
        //isThrowing = true;
        isStopping = false;
        isDashing = false;
        buffer = 0;
        targetLocation = transform.position + (target/rb.mass);
        targetLocation.y = transform.position.y;
        //Debug.DrawRay(transform.position, target / rb.mass, Color.gray, 5f);
        dir = targetLocation - transform.position;
        dir.y = 0;
        speed = force/rb.mass;
        isLaunched = true;
        IgnorePlayer();
    }

    public void Slammed(Vector3 target, float force, Collider collider)
    {
        isStopping = false;
        isDashing = false;
        buffer = 0;
        targetLocation = transform.position + (target / rb.mass);
        targetLocation.y = transform.position.y;
        //Debug.DrawRay(transform.position, target / rb.mass, Color.gray, 5f);
        dir = targetLocation - transform.position;
        dir.y = 0;
        speed = force / rb.mass;
        isLaunched = true;
        collidersHit.Add(collider);
        Physics.IgnoreCollision(myCollider, collider, true);
    }
    
    void IgnorePlayer()
    {
        if (!collidersHit.Contains(playerCollider))
        {
            collidersHit.Add(playerCollider);
            Physics.IgnoreCollision(myCollider, playerCollider, true);
        }
    }

    public void Tossed(float force)
    {
        speed += force / rb.mass;
    }

    public void Hold()
    {
        hold = true;
        isStopping = false;
        isDashing = false;
        buffer = 0;
        targetLocation = transform.position;
        dir = Vector3.zero;
        speed = 0;
        isLaunched = true;
        IgnorePlayer();
    }

    public void Dash(Vector3 target, float time)
    {
        hold = false;
        isStopping = false;
        isDashing = true;
        buffer = 0;
        targetLocation = transform.position + target;
        targetLocation.y = transform.position.y;
        dir = targetLocation - transform.position;
        dir.y = 0;
        speed = Vector3.Distance(transform.position, targetLocation) / time;
        isLaunched = true;
    }

    public void Ram(Vector3 target, float time)
    {
        hold = false;
        isStopping = false;
        isDashing = true;
        buffer = 0;
        targetLocation = transform.position + target;
        targetLocation.y = transform.position.y;
        dir = targetLocation - transform.position;
        dir.y = 0;
        speed = Vector3.Distance(transform.position, targetLocation) / time;
        isLaunched = true;
        unstoppable = true;
    }

    private bool IsGrounded()
    {
        Debug.DrawRay(col.bounds.center, Vector3.down * CalculateDown(), Color.green, 2f);
        return Physics.Raycast(col.bounds.center, Vector3.down, CalculateDown(), groundLayers);
    }

    private float CalculateDown()
    {
        float angle1 = Vector3.Angle(Vector3.down, transform.up)%90;
        float angle2 = Vector3.Angle(Vector3.down, transform.right)%90;
        float angle3 = Vector3.Angle(Vector3.down, transform.forward)%90;

        float y = Mathf.Cos(angle1) * boundsY;
        float x = Mathf.Cos(angle2) * boundsX;
        float z = Mathf.Cos(angle3) * boundsZ;

        //Debug.Log(x);
        //Debug.Log(y);
        //Debug.Log(z);
        return Vector3.Magnitude(new Vector3(x,y,z)); 
    }

    public bool TriggerRelease()
    {
        if(!isLaunched)
        {
            return true;
        }
        //Debug.Log(!(stopping == null));
        return !(stopping == null);
    }

    public void ForceRelease()
    {
        tendrilOwner = null;
        //isThrowing = false;
        stopping = StartCoroutine(Tumbling());
    }

    public void ForceReleaseDelayed()
    {
        StartCoroutine(ReleaseDelayed());
    }

    IEnumerator ReleaseDelayed()
    {
        for(int i = 0; i<2; i++) 
        {
            yield return new WaitForFixedUpdate();
        }
        ForceRelease();
    }
}
