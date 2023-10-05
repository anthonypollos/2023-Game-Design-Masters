using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LassoBehavior : MonoBehaviour
{
    private GameController gc;
    public IsoAttackManager attackManager;
    [HideInInspector]
    public float maxDistance = 999;
    private GameObject attached;
    private Rigidbody attachedRB;
    private bool grounded;
    private Vector3 startingPos;
    private Quaternion startingRot;
    private Vector3 forwardVector;
    private Vector3 rightVector;
    private Vector3 leftVector;
    private LassoRange lassoRange;
    private float adjustedPullRange;
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
        attackManager=FindObjectOfType<IsoAttackManager>();
        lassoRange = GetComponentInChildren<LassoRange>();
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
        gc = FindObjectOfType<GameController>();
        //Handles.color = Color.cyan;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject temp = collision.gameObject;
        if (attached == null && !grounded)
        {
            if (temp.CompareTag("Ground") || temp.CompareTag("Wall"))
            {
                Destroy(gameObject);
            }
            else if (temp.GetComponent<IPullable>() != null)
            {
                attached = temp;
                forwardVector = (player.position - attached.transform.position).normalized;
                attached.GetComponentInParent<IPullable>().Lassoed();
                Physics.IgnoreCollision(GetComponent<Collider>(), temp.GetComponent<Collider>(), true);
                gameObject.transform.parent = temp.transform;
                transform.localPosition = Vector3.zero;
                gameObject.GetComponent<Rigidbody>().isKinematic = true;

                moveable = temp.GetComponent<Moveable>();
                if (moveable != null)
                {
                    lassoRange.SetAttached(attached.transform);
                    lr.enabled = true;
                    attachedRB = temp.GetComponent<Rigidbody>();
                    adjustedPullRange = pullDistance / attachedRB.mass;
                }
                if (gc.toggleLasso)
                {
                    dir = forwardVector;
                    attackManager.Pull(this);
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
        
        if(moveable!=null && !gc.toggleLasso)
        {
            
            if (!CheckAngle())
                if(IsRight())
                {
                    dir = rightVector;
                }
                else
                {
                    dir = leftVector;
                }
            dir.y = 0;
            Vector3[] positions = { attached.transform.position, attached.transform.position + dir * adjustedPullRange};
            lassoRange.SetRangeArc(forwardVector, pullAngle, adjustedPullRange);
            lr.SetPositions(positions);
        }
    }

    public (bool success, Vector3 position) GetMousePosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            Debug.DrawRay(hitInfo.point, Vector3.down * 10, Color.red, 10f);
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
        if (InputChecker.instance.IsController())
        {
            var direction = Helpers.ToIso(attackManager.pc._aimInput);
            direction.y = 0;
            dir = direction.normalized;

        }
        else {
            bool check;
            Vector3 mouseVector;
            (check, mouseVector) = GetMousePosition();
            if (check)
            {
                var direction = mouseVector - attached.transform.position;
                direction.y = 0;
                dir = direction.normalized;
            }
            else return false;
        }
            float angle = Vector3.Angle(forwardVector.normalized, dir);
            return (angle <= pullAngle);

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
