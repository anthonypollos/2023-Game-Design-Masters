using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LassoBehavior : MonoBehaviour
{
    private GameController gc;
    public IsoAttackManager attackManager;
    private float maxDistance = 999;
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
    private float maxPullDistance;
    private float minPullDistance;
    private float calculatedDistance;
    [SerializeField] float pullAngle = 90f;
    private Transform player;
    private Vector3 dir;

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

    public void SetValues(float maxPullDistance, float minModifier, float maxRange, Transform playerPos)
    {
        this.maxPullDistance = maxPullDistance;
        this.minPullDistance = maxPullDistance * minModifier;
        this.maxDistance = maxRange;
        this.player = playerPos;
    }

    public (Vector3, float) GetValues()
    {
        return (dir, calculatedDistance);
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
                    attachedRB = temp.GetComponent<Rigidbody>();
                    lassoRange.SetAttached(attached.transform, attachedRB);
                    lr.enabled = true;
                    
                    adjustedPullRange = maxPullDistance / attachedRB.mass;
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

            float angle = CheckAngle();
            calculatedDistance = Mathf.Lerp(maxPullDistance, minPullDistance, angle/180)/attachedRB.mass;
            //(maxPullDistance - ((maxPullDistance - minPullDistance) / 180) * Mathf.Abs(angle)) / attachedRB.mass
            dir.y = 0;
            Vector3[] positions = { attached.transform.position, attached.transform.position + dir * calculatedDistance};
            lassoRange.SetRangeArc(forwardVector, maxPullDistance, minPullDistance);
            lr.SetPositions(positions);
            //Debug.Log(calculatedDistance);
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

    private float CheckAngle()
    {
        forwardVector = (player.position - attached.transform.position).normalized;
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
            else return 0;
        }
            float angle = Vector3.Angle(forwardVector.normalized, dir);
        return angle;

    }


    public (GameObject,Moveable) GetAttachment()
    {
        return (attached, moveable);
    }
}
