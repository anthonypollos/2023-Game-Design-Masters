using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGizmos : MonoBehaviour
{
    [SerializeField]
    private Color colliderColor;

    [SerializeField] SphereCollider col;

    public enum GizmoShape { sphere, cube }
    [SerializeField] GizmoShape gizmoShape;

    private void OnDrawGizmos()
    {
        Gizmos.color = colliderColor;
        Gizmos.matrix = transform.localToWorldMatrix;

        if(gizmoShape == GizmoShape.sphere)
            Gizmos.DrawSphere(Vector3.zero, col.radius);

        else if(gizmoShape == GizmoShape.cube)
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }
}
