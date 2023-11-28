using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TendrilTest : MonoBehaviour
{
    [SerializeField] Transform target;

    [SerializeField] bool isStart;

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
        Vector3 dir = target.position - transform.position;
        dir = dir.normalized;
        if (dir != Vector3.zero)
        {
            transform.forward = Quaternion.Euler(rotateOffset) * (target.position - transform.position).normalized;
          
        }
    }
}
