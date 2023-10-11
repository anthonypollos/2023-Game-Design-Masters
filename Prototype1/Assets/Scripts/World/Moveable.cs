using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveable : MonoBehaviour
{
    Rigidbody rb;
    Vector3 targetLocation;
    float speed;
    [HideInInspector] public bool isLaunched;
    Coroutine stopping;
    float buffer;
    // Start is called before the first frame update
    void Start()
    {
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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isLaunched)
        {
            Vector3 dir = targetLocation - transform.position;
            dir.y = 0;
            rb.velocity = dir.normalized * speed + Vector3.up * Mathf.Clamp(rb.velocity.y, Mathf.NegativeInfinity, 1);
            Vector3 positionIgnoreY = transform.position;
            positionIgnoreY.y = 0;
            Vector3 targetIgnoreY = targetLocation;
            targetIgnoreY.y = 0;
            if(Vector3.Distance(positionIgnoreY, targetIgnoreY) < 0.5f)
            {
                //Debug.Log("Reached Destination");
                if(stopping==null)
                    stopping = StartCoroutine(Stop());
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(!collision.transform.CompareTag("Ground") && isLaunched && buffer>.5f)
        {
            Debug.Log(collision.gameObject.name);
            Debug.Log("Hit object");
            if (stopping == null)
                stopping = StartCoroutine(Stop());
        }
    }

    private IEnumerator Stop()
    {
        rb.velocity = Vector3.zero + Vector3.up * rb.velocity.y;
        yield return new WaitForSeconds(0.01f);
        isLaunched = false;
        rb.velocity = Vector3.zero + Vector3.up * rb.velocity.y;
        stopping = null;
    }

    public void Launched(Vector3 target, float force)
    {
        buffer = 0;
        targetLocation = transform.position + (target/rb.mass);
        targetLocation.y = transform.position.y;
        Debug.DrawRay(transform.position, target / rb.mass, Color.gray, 5f);
        speed = force;
        isLaunched = true;
    }

    public void Dash(Vector3 target, float time)
    {
        buffer = 0;
        targetLocation = transform.position + target;
        targetLocation.y = transform.position.y;
        speed = Vector3.Distance(transform.position, targetLocation) / time;
        isLaunched = true;
    }
}
