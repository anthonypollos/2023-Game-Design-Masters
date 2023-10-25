using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LassoBehavior : MonoBehaviour
{
    private GameController gc;
    public IsoAttackManager attackManager;
    private Collider collider;

    private float maxThrowDistance = 999;
    private float maxDistance = 999;
    private GameObject attached;
    private Rigidbody attachedRB;
    private bool grounded;
    private Vector3 startingPos;
    private Quaternion startingRot;
    private Vector3 forwardVector;
    private Vector3 rightVector;
    private Vector3 leftVector;
    //private LassoRange lassoRange;
    private float adjustedPullRange;
    private float maxPullDistance;
    private float minPullDistance;
    private float calculatedDistance;
    [SerializeField] [Tooltip("Set to 0 for old version, else this takes over")]float trajectoryArrowDistance = 0f;
    //[SerializeField] float pullAngle = 90f;
    private Transform player;
    private Vector3 dir;
    private Slider slider;
    private Image sliderFill;

    private Moveable moveable;
    private Camera cam;
    private LineRenderer lr;
    [SerializeField] private LayerMask groundMask;

    LassoLine line;
    // Start is called before the first frame update
    private void Start()
    {
        collider = GetComponent<Collider>();
        attackManager = FindObjectOfType<IsoAttackManager>();
        //lassoRange = GetComponentInChildren<LassoRange>();
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
        line = GetComponentInChildren<LassoLine>();
        line.gameObject.SetActive(false);
        player = attackManager.transform;
        //Handles.color = Color.cyan;
    }

    public void SetValues(float maxPullDistance, float minModifier, float maxThrowRange, float breakRange, Slider slider, Image sliderFill)
    {
        moveable = null;
        attached = null;
        startingPos = transform.position;
        this.maxPullDistance = maxPullDistance;
        this.minPullDistance = maxPullDistance * minModifier;
        this.maxThrowDistance = maxThrowRange;
        //this.player = playerPos;
        this.slider = slider;
        this.maxDistance = breakRange;
        line.SetValues(player, maxDistance);
        line.gameObject.SetActive(true);
        this.sliderFill = sliderFill;

    }

    public (Vector3, float) GetValues()
    {
        return (dir, calculatedDistance * attachedRB.mass);
    }

    public void Launched()
    {
        line.gameObject.SetActive(true);
        slider.value = 0f;
        slider.gameObject.SetActive(true);
        collider.enabled = true;
    }

    private void OnTriggerEnter(Collider collision)
    {
        GameObject temp = collision.gameObject;
        if (attached == null && !grounded)
        {
            if (temp.CompareTag("Ground") || temp.CompareTag("Wall"))
            {
                attackManager.Release();
            }
            else if (temp.GetComponentInParent<IPullable>() != null)
            {
                attached = temp;
                forwardVector = (player.position - attached.transform.position).normalized;
                attached.GetComponentInParent<IPullable>().Lassoed();
                //Physics.IgnoreCollision(GetComponent<Collider>(), temp.GetComponent<Collider>(), true);
                gameObject.transform.parent = temp.transform;
                transform.localPosition = Vector3.zero;
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
                slider.value = 0;
                slider.gameObject.SetActive(true);
                moveable = temp.GetComponent<Moveable>();
                if (moveable != null)
                {
                    moveable.tendrilOwner = attackManager;
                    attachedRB = temp.GetComponent<Rigidbody>();
                    //lassoRange.SetAttached(attached.transform, attachedRB);
                    lr.enabled = true;
                    line.gameObject.SetActive(true);

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

    /*private void OnTriggerEnter(Collider other)
    {
        GameObject temp = other.gameObject;
        if(attached == null && !grounded)
        {
            if(temp.GetComponentInParent<IPullable>() != null)
            {
                attached = temp;
                attached.GetComponentInParent<IPullable>().Lassoed();
                Physics.IgnoreCollision(GetComponent<Collider>(), temp.GetComponent<Collider>(), true);
                gameObject.transform.parent = temp.transform;
                transform.localPosition = Vector3.zero;
                slider.value = 0;
                slider.gameObject.SetActive(true);
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    } */

    private void Update()
    {
        if (Vector3.Distance(startingPos, transform.position) >= maxThrowDistance && attached == null) attackManager.Release();
        float distance = Vector3.Distance(transform.position, player.position);
        slider.value = distance / maxDistance;
        sliderFill.color = line.GetGradient().Evaluate(distance / maxDistance);

        if (distance > maxDistance)
        {
            slider.gameObject.SetActive(false);
            if (attached != null)
                attached.GetComponent<IPullable>().Break();
            attackManager.ForceRelease();
            if(moveable != null)
                moveable.ForceRelease();
            moveable = null;
            attached = null;
        }

        if (moveable != null && !gc.toggleLasso)
        {

            float angle = CheckAngle();
            calculatedDistance = trajectoryArrowDistance == 0f ? Mathf.Lerp(maxPullDistance, minPullDistance, angle / 180) / attachedRB.mass : trajectoryArrowDistance;
            //(maxPullDistance - ((maxPullDistance - minPullDistance) / 180) * Mathf.Abs(angle)) / attachedRB.mass
            dir.y = 0;
            Vector3[] positions = { attached.transform.position, attached.transform.position + dir * calculatedDistance };
            //lassoRange.SetRangeArc(forwardVector, maxPullDistance, minPullDistance);
            lr.SetPositions(positions);
            //Debug.Log(calculatedDistance);
        }

        if (attached != null && !attached.activeInHierarchy)
        {
            attackManager.ForceRelease();
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
        else
        {
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

    private void OnDestroy()
    {
        slider.gameObject.SetActive(false);
    }


    public (GameObject, Moveable) GetAttachment()
    {
        return (attached, moveable);
    }

    public bool TriggerRelease()
    {
        bool returnValue = true;
        transform.parent = null;
        lr.enabled = true;
        line.gameObject.SetActive(true);
        if (moveable != null && moveable.gameObject.activeInHierarchy)
        {
            //Debug.Log("movable avalible");
            returnValue = moveable.TriggerRelease();
            if (!returnValue)
                transform.position = moveable.transform.position;
            else
            {
                if(moveable != null)
                    moveable.tendrilOwner = null;
                moveable = null;
            }
                
                
        }
        //Debug.Log(returnValue);
        return returnValue;
    }

    public void Retracted()
    {
        lr.enabled = false;
        line.gameObject.SetActive(false);
        slider.gameObject.SetActive(false);

    }

    public void StartRetracting()
    {
        if(attached!=null)
        {
            attached.GetComponent<IPullable>().Break();
        }
        if(moveable!=null)
        {
            moveable.tendrilOwner = null;
        }
        moveable = null;
        attached = null;
        lr.enabled = false;
        collider.enabled = false;
    }


}
