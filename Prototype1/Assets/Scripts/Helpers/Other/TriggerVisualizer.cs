using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * 
 * Purpose: Draws a visible cube for trigger objects
 * Author: Alex Strook and Sean Lee
 * 
 */

public class TriggerVisualizer : MonoBehaviour
{

    public Color _myColor; //color

    //Destroy Mesh Renderer if it exists
    void Awake()
    {
        if (GetComponent<MeshRenderer>() != null)
        {
            Destroy(GetComponent("MeshRenderer"));
        }
    }

    //Gizmo cube if you have gizmos enabled
    private void OnDrawGizmos()
    {
        Gizmos.color = _myColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }

}
