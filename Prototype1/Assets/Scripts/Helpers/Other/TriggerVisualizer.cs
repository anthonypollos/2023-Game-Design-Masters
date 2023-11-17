using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * 
 * Purpose: Draws a visible cube for trigger objects
 * Author: Alex Strook
 * 
 */

public class TriggerVisualizer : MonoBehaviour
{

    public Color _myColor; //color

    private void OnDrawGizmos()
    {
        Gizmos.color = _myColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }

}
