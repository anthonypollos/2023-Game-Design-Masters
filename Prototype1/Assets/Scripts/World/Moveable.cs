using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneTemplate;
using UnityEngine;

public class Moveable : MonoBehaviour
{
    Rigidbody rb;
    Vector3 targetLocation;
    float speed;
    [HideInInspector] public bool isLaunched;
    Coroutine stopping;
    float buffer;
    [SerializeField] float timeToStop = 1f;
    bool isDashing;
    LayerMask groundLayers;
    [SerializeField] float groundCheckBuffer = 0.1f;
    float boundsY;
    float boundsX;
    float boundsZ;
    Collider col;
    bool isStopping = false;
    bool isThrowing = false;
    Vector3 dir;

    // Start is called before the first frame update
    void Start()
    {

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
            
            rb.velocity = dir.normalized * speed + Vector3.up * Mathf.Clamp(rb.velocity.y, Mathf.NegativeInfinity, 1);
            Vector3 positionIgnoreY = transform.position;
            positionIgnoreY.y = 0;
            Vector3 targetIgnoreY = targetLocation;
            targetIgnoreY.y = 0;
            if(Vector3.Distance(positionIgnoreY, targetIgnoreY) < 0.5f)
            {
                isThrowing = false;
                if (isDashing)
                    stopping = StartCoroutine(Stop());
                else
                    stopping = StartCoroutine(Tumbling());
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(!collision.transform.CompareTag("Ground") && isLaunched && buffer>.5f)
        {
            //Debug.Log(collision.gameObject.name);
            //Debug.Log("Hit object");
            if (!isStopping)
                stopping = StartCoroutine(Stop());
        }
    }

    private IEnumerator Tumbling()
    {
        float time = 0;
        Vector3 startingVelocity = rb.velocity;
        startingVelocity.y = 0;
        while(time < timeToStop)
        {
            yield return new WaitForSeconds(0.05f);
            //yield return new WaitUntil(IsGrounded);
            time += 0.05f;
            //Debug.Log("Starting Vel:" + startingVelocity);
            //Debug.Log("Calculated subtraction: " + -startingVelocity * time / timeToStop);
            //Debug.Log("Calculated Vel:"+ (startingVelocity - startingVelocity * time / timeToStop));
            //Debug.DrawRay(transform.position, (startingVelocity - startingVelocity * time / timeToStop));
            rb.velocity = startingVelocity - (startingVelocity * time / timeToStop) + Vector3.up * rb.velocity.y;
        }
        stopping = StartCoroutine (Stop());
    }

    private IEnumerator Stop()
    {
        isStopping = true;
        rb.velocity = Vector3.zero + Vector3.up * rb.velocity.y;
        yield return new WaitForSeconds(0.01f);
        //yield return new WaitUntil(IsGrounded);
        isLaunched = false;
        rb.velocity = Vector3.zero + Vector3.up * rb.velocity.y;
        stopping = null;
        isStopping = false;
    }

    public void Launched(Vector3 target, float force)
    {
        isThrowing = true;
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

    }

    public void Dash(Vector3 target, float time)
    {
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
        Debug.Log(!(stopping == null));
        return !(stopping == null);
    }

    public void ForceRelease()
    {
        isThrowing = false;
        stopping = StartCoroutine(Tumbling());
    }
}
