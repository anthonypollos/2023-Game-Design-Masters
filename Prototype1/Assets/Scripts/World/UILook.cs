using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILook : MonoBehaviour
{
    private void LateUpdate()
    {
        Transform target = transform.parent;

        transform.rotation = Quaternion.Euler(-50, (-target.rotation.y + 180), 0);

        //transform.LookAt(Camera.main.transform);
    }
}
