using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoBehavior : MonoBehaviour
{
    public float maxDistance = 999;
    private GameObject attached;
    private Rigidbody attachedRB;
    private bool grounded;
    private Vector3 startingPos;
    private Quaternion startingRot;
    private Vector3 forwardVector;
    private Vector3 rightVector;
    private Vector3 leftVector;
    [SerializeField] float pullAngle = 90f;
    [HideInInspector]public Transform player;
    [HideInInspector]public Vector3 dir;
    [HideInInspector]public float pullDistance;

    private Moveable moveable;
    private Camera cam;
    private LineRenderer lr;
    [SerializeField] private LayerMask groundMask;
    // Start is called before the first frame update
    private void Start()
    {
        grounded = false;
        attached = null;
        attachedRB = null;
        moveable = null;
        startingPos = transform.position;
        startingRot = transform.rotation;
        dir = Vector3.zero;
        cam = Camera.main;
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject temp = collision.gameObject;
        if (attached == null && !grounded)
        {
            if (temp.CompareTag("Ground") || temp.CompareTag("Wall"))
            {
                grounded = true;
                gameObject.GetComponent<Rigidbody>().useGravity = true;
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            else if (temp.GetComponent<IPullable>() != null)
            {
                attached = temp;
                attached.GetComponentInParent<IPullable>().Lassoed();
                Physics.IgnoreCollision(GetComponent<Collider>(), temp.GetComponent<Collider>(), true);
                gameObject.transform.parent = temp.transform;
                transform.localPosition = Vector3.zero;
                gameObject.GetComponent<Rigidbody>().isKinematic = true;

                moveable = temp.GetComponent<Moveable>();
                if (moveable != null)
                {
                    lr.enabled = true;
                    attachedRB = temp.GetComponent<Rigidbody>();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject temp = other.gameObject;
        if(attached == null && !grounded)
        {
            if(temp.GetComponent<IPullable>() != null)
            {
                attached = temp;
                attached.GetComponentInParent<IPullable>().Lassoed();
                Physics.IgnoreCollision(GetComponent<Collider>(), temp.GetComponent<Collider>(), true);
                gameObject.transform.parent = temp.transform;
                transform.localPosition = Vector3.zero;
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }

    private void Update()
    {
        if (Vector3.Distance(startingPos, transform.position) >= maxDistance && attached==null) Destroy(gameObject);

        if(moveable!=null)
        {
            if(!CheckAngle())
                if(IsRight())
                {
                    dir = rightVector;
                }
                else
                {
                    dir = leftVector;
                }
            dir.y = 0;
            Vector3[] positions = { attached.transform.position, attached.transform.position + dir * pullDistance/attachedRB.mass};
            lr.SetPositions(positions);
        }
    }

    public (bool success, Vector3 position) GetMousePosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            return (success: true, position: hitInfo.point);

        }
        else
        {
            return (success: false, position: Vector3.zero);
        }
    }

    private bool CheckAngle()
    {
        forwardVector = (player.position - attached.transform.position).normalized;
        rightVector =  Quaternion.Euler(new Vector3(0, -pullAngle, 0)) * forwardVector;
        leftVector = Quaternion.Euler(new Vector3(0, pullAngle, 0)) * forwardVector;
        //Debug.DrawRay(attached.transform.position, forwardVector, Color.black);
        //Debug.DrawRay(attached.transform.position, rightVector, Color.green);
        //Debug.DrawRay(attached.transform.position, -rightVector, Color.red);
        bool check;
        Vector3 mouseVector;
        (check, mouseVector) = GetMousePosition();
        if(check)
        {
            mouseVector = mouseVector - attached.transform.position;
            dir = mouseVector.normalized;
            float angle = Vector3.Angle(forwardVector.normalized, mouseVector.normalized);
            return (angle <= pullAngle);
        }
        else
        {
            return false;
        }
    }

    private bool IsRight()
    {
        float angle1 = Vector3.Angle(rightVector, dir);
        float angle2 = Vector3.Angle(leftVector, dir);
        return (angle1 <= angle2);

    }

    public (GameObject,Moveable) GetAttachment()
    {
        return (attached, moveable);
    }
}
