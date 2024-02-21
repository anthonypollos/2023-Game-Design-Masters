using Ink.Parsed;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
//using UnityEditor.UIElements;
//using UnityEditor.SceneTemplate;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEngine.UI.Image;

public class Moveable : MonoBehaviour, ISlowable
{

    [SerializeField] int clashDamage = 10;
    [SerializeField] float neutralSpeed = 15f;
    [SerializeField] float rateOfChange = 1f;
    [SerializeField] float maxDamage = 40f;

    Rigidbody rb;
    Vector3 targetLocation;
    [SerializeField ] float speed;
    [HideInInspector] public bool isLaunched;
    Coroutine stopping;
    float buffer;
    bool initialPull;
    //[SerializeField] float timeToStop = 1f;
    [SerializeField] float slowdownPerSecond = 30f;
    [HideInInspector]
    public bool isDashing;
    LayerMask groundLayers;
    //[SerializeField] float groundCheckBuffer = 0.1f;
    bool isStopping = false;
    bool hold = false;
    //bool isThrowing = false;
    [HideInInspector]
    public IsoAttackManager tendrilOwner;
    Vector3 dir;
    bool unstoppable = false;
    List<Collider> collidersHit;
    Collider myCollider;
    List<Collider> myColliders;
    [SerializeField] GameObject flyingHitBox;
    Collider playerCollider;
    IDamageable myDamageable;
    private bool charged = false;
    private GameObject chargedDetonationPrefab;

    List<float> slowMods;
    float[] slowModsArray;

    [SerializeField] float flyingHitBoxMod = 1.25f;
    [SerializeField] bool debug = false;

    GenericItem gi;

    // Start is called before the first frame update
    void Start()
    {
        initialPull = false;
        gi = GetComponent<GenericItem>();
        EnterSlowArea(0);
        myDamageable = GetComponent<IDamageable>();
        myCollider = GetComponent<Collider>();
        myColliders = new List<Collider>(GetComponentsInChildren<Collider>());
        playerCollider = GameController.GetPlayer().GetComponent<Collider>();
        collidersHit = new List<Collider>();
        hold = false;
        string[] temp = { "Ground", "Ground_Transparent" };
        groundLayers = LayerMask.GetMask(temp);
        stopping = null;
        rb = GetComponent<Rigidbody>();
        targetLocation = transform.position;
        speed = 0;
        isLaunched = false;
        buffer = 0;
        GenerateHitbox();
       
    }

    void GenerateHitbox()
    {
        if (flyingHitBox == null)
        {
            System.Type type = myCollider.GetType();
            flyingHitBox = new GameObject("FlyingHitBox");
            flyingHitBox.transform.parent = transform;
            flyingHitBox.transform.localPosition = Vector3.zero;
            Collider col = flyingHitBox.AddComponent(myCollider);
            flyingHitBox.name = "FlyingHitbox";
            flyingHitBox.transform.localRotation = Quaternion.identity;
            
            flyingHitBox.transform.localScale = Vector3.one * flyingHitBoxMod;
            col.isTrigger = true;
            myColliders.Add(col);
            flyingHitBox.layer = LayerMask.NameToLayer("Thrown");
            flyingHitBox.SetActive(false);
        }
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

    public virtual void OnClash()
    {
        if(charged && chargedDetonationPrefab!= null)
        {
            Instantiate(chargedDetonationPrefab, transform.position, Quaternion.identity);
            charged = false;
        }
    }

    public float GetMass()
    {
        return rb.mass;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.transform.CompareTags(StoredValues.MovableTagsToIgnore) && isLaunched && buffer>.5f)
        {
            if (collidersHit.Contains(collision.collider))
                return;
            Debug.Log("Broke on " + collision.gameObject.name);
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
        if (!collision.transform.CompareTags(StoredValues.MovableTagsToIgnore) && isLaunched && (!isDashing || unstoppable) && 
            (stopping != null || buffer>0.2f))
        {
            Collider[] colliders = collision.gameObject.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                if (collidersHit.Contains(collider))
                    return;
                foreach (Collider myCollider in myColliders)
                {
                    collidersHit.Add(collider);
                    Physics.IgnoreCollision(myCollider, collider, true);
                }
            }

            //tendril lets go
            if (tendrilOwner != null)
            {
                //Debug.Log("Force release");
                tendrilOwner.ForceRelease();
                tendrilOwner = null;
            }
            Moveable moveable = collision.transform.GetComponent<Moveable>();
            OnClash();
            //calculate clash damage
            if (!unstoppable)
            {
                if (moveable == null)
                    myDamageable.TakeDamage(CalculateClashDamage());
                else if (!moveable.AlreadyHit(myCollider))
                {
                    myDamageable.TakeDamage(CalculateClashDamage());
                    moveable.OnClash();
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
                        int coin = UnityEngine.Random.Range(0, 2);
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
                if(gameObject.activeInHierarchy)
                    stopping = StartCoroutine(Stop());
            }
            
            //Check for Trap
            ITrap trap = collision.transform.GetComponent<ITrap>();
            if(trap != null)
            {
                trap.ActivateTrap(gameObject);
            }
            if (gi != null)
            {
                gi.OnHitModifier(collision.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (!collision.transform.CompareTags(StoredValues.MovableTagsToIgnore) && isLaunched && buffer > .5f)
        {
            if (collidersHit.Contains(collision))
                return;
            Debug.Log("Broke on" +  collision.gameObject.name);
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

    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("trigger activated on " + collision.name);
        //Debug.Log("boolean checks: " + (stopping != null) + " "  + (buffer > 0.2f));
        if (!collision.transform.CompareTags(StoredValues.MovableTagsToIgnore) && isLaunched && (!isDashing || unstoppable) && 
            (stopping !=null || buffer > 0.2f))
        {
            //Debug.Log("trigger calculations going on " + collision.name);
            Collider[] colliders = collision.gameObject.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                if (collidersHit.Contains(collider))
                    return;
                foreach (Collider myCollider in myColliders)
                {
                    collidersHit.Add(collider);
                    Physics.IgnoreCollision(myCollider, collider, true);
                }
            }

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
                    if (!unstoppable)
                    {
                        Vector3 dir = collision.transform.position - collision.ClosestPointOnBounds(transform.position);
                        dir.y = 0;
                        dir = dir.normalized;
                        moveable.Slammed(dir, rb.mass * speed / 2, myCollider);
                        speed /= 2;
                        ForceRelease();
                    }
             
                    IKickable kickable = collision.transform.GetComponent<IKickable>();
                    if (kickable != null)
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
                if (at != null)
                {
                    at.ForceAnimationChange();
                }
                if (gameObject.activeInHierarchy)
                    stopping = StartCoroutine(Stop());
            }

            //Check for Trap
            ITrap trap = collision.transform.GetComponent<ITrap>();
            if (trap != null)
            {
                trap.ActivateTrap(gameObject);
            }
            if (gi != null)
            {
                gi.OnHitModifier(collision.gameObject);
            }
        }
    }

    int CalculateClashDamage()
    {
        return Mathf.RoundToInt(Mathf.Clamp(((float)clashDamage + (speed - neutralSpeed)*rateOfChange), 0, maxDamage));
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
            float modified = slowdownPerSecond * (1+Mathf.Max(slowModsArray));
            speed = Mathf.Clamp(speed - (modified * 0.01f), 0, speed);
            //Debug.Log(speed + "-" + slowPerSecond * 0.01f + "=" + (speed - (slowPerSecond * 0.01f)));
            //Debug.Log("Speed = " + speed);
            rb.velocity = dir * speed + Vector3.up * rb.velocity.y;
        }

        stopping = StartCoroutine (Stop());
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

    private IEnumerator Stop()
    {
        buffer = 0;
        initialPull = false;
        flyingHitBox.SetActive(false);
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
        isDashing = false;
        ResetCollisions();
    }

    void ResetCollisions()
    {
        foreach(Collider collider in collidersHit)
        {
            foreach (Collider myCollider in myColliders)
            {
                if (collider != null)
                    Physics.IgnoreCollision(myCollider, collider, false);
            }
        }
        collidersHit.Clear();
    }

    public void Launched(Vector3 target, float force)
    {
        //isThrowing = true;
        flyingHitBox.SetActive(true);
        isStopping = false;
        isDashing = false;
        if (!initialPull)
        {
            initialPull = true;
            buffer = 0;
        }
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
        if (unstoppable)
            return;
        flyingHitBox.SetActive(false);
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
            foreach(Collider myCollider in myColliders)
                Physics.IgnoreCollision(myCollider, playerCollider, true);
        }
    }

    public void Tossed(float force)
    {
        if(speed!=0)
            speed += force / rb.mass;
    }

    public void Hold()
    {
        if(stopping!=null)
        {
            StopCoroutine(stopping);
            stopping = null;
        }
        flyingHitBox.SetActive(false);
        hold = true;
        isStopping = false;
        isDashing = false;
        if (!initialPull)
        {
            buffer = 0;
        }
        targetLocation = transform.position;
        dir = Vector3.zero;
        speed = 0;
        isLaunched = true;
        IgnorePlayer();
    }

    public void Grabbed()
    {
        if (stopping != null)
        {
            StopCoroutine(stopping);
            stopping = null;
        }
        flyingHitBox.SetActive(false);
        hold = true;
        isStopping = false;
        isDashing = false;
        buffer = 0;
        targetLocation = transform.position;
        dir = Vector3.zero;
        speed = 0;
        isLaunched = false;
        IgnorePlayer();
        float timer;
        var temp = tendrilOwner.TakeCharge();
        if (!charged)
        {
            charged = temp.Item1;
        }
        chargedDetonationPrefab = temp.Item2;
        timer = temp.Item3;
        if(temp.Item1 && timer > 0)
        {
            StopCoroutine("ChargeCountdown");
            StartCoroutine(ChargeCountdown(timer));
        }
    }
    private IEnumerator ChargeCountdown(float timer)
    {
        yield return new WaitForSeconds(timer);
        charged = false;
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

    public void ForceStop()
    {
        if(stopping ==null)
        {
            stopping = StartCoroutine(Stop());
        }
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
        if (gameObject.activeInHierarchy)
        { 
            stopping = StartCoroutine(Tumbling());
        }
    }

    public void ForceReleaseDelayed()
    {
        if(gameObject.activeInHierarchy)
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
