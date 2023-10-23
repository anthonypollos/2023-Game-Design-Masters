using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TendrilTest : MonoBehaviour
{
    public Transform target;

    public float rotateSpeed = 10000;

    public Vector3 rotateOffset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateToTarget();
    }

    private void RotateToTarget()
    {
        // root rotate to face target
        var q = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotateSpeed * Time.deltaTime);
    }
}
