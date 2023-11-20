using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  RotateTowards.cs
 *  Purpose: Makes an object smoothly rotate towards another object.
 *  Author: Sean Lee (from https://forum.unity.com/threads/locking-the-x-and-z-axis-while-using-transform-lookat.521817/)
 */

public class RotateTowards : MonoBehaviour
{
    public Transform Target;
    private void Update()
    {
        Vector3 dir = Target.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
    }

}
