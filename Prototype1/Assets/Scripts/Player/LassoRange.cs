using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoRange : MonoBehaviour
{
    LineRenderer lr;
    Transform attached;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
        attached = null;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAttached(Transform attached)
    {
        this.attached = attached;
    }

    public void SetRangeArc(Vector3 forwardVector, float pullAngle, float range)
    {
        List<Vector3> positions = new List<Vector3>();
        lr.positionCount = (int)pullAngle * 2 + 2;
        positions.Add(attached.position);
        for(float i =  -pullAngle; i<=pullAngle; i++)
        {
            Vector3 dir = (Quaternion.Euler(0, i, 0) * forwardVector).normalized;
            dir.y = 0;
            positions.Add(attached.position + dir*range);
        }
        Debug.Log(positions.Count);
        lr.SetPositions(positions.ToArray());
        lr.enabled = true;
        
    }
}
