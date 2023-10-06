using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoRange : MonoBehaviour
{
    LineRenderer lr;
    Transform attached;
    Rigidbody rb;
    // Start is called before the first frame update
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
        attached = null;
        rb = null;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAttached(Transform attached, Rigidbody rb)
    {
        this.attached = attached;
        this.rb = rb;
    }

    public void SetRangeArc(Vector3 forwardVector, float maxRange, float minRange)
    {
        List<Vector3> positions = new List<Vector3>();
        lr.positionCount = 180 * 2 + 1;
        for(float i =  -180; i<=180; i++)
        {
            Vector3 dir = (Quaternion.Euler(0, i, 0) * forwardVector).normalized;
            dir.y = 0;
            float calculatedRange = Mathf.Lerp(maxRange, minRange, Mathf.Abs(i)/180)/rb.mass;
            //(maxRange - ((maxRange - minRange) / 180) * Mathf.Abs(i)) / rb.mass
            positions.Add(attached.position + dir*calculatedRange);
        }
        //Debug.Log(positions.Count);
        lr.SetPositions(positions.ToArray());
        lr.enabled = true;
        
    }
}
