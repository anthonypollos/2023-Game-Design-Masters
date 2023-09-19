using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoAttackManager : MonoBehaviour, ICanKick
{
    [Header("Lasso properties")]
    [SerializeField] [Tooltip("The speed of the lasso")] float lassoSpeed;
    [SerializeField] [Tooltip("The force of the pull")] float pullForce;
    [SerializeField] [Tooltip("The time it takes for full charge")] float lassoChargeTime;
    [SerializeField] [Tooltip("Minimum distance of lasso")] float minLassoDistance;
    [SerializeField] [Tooltip("Maximum distance of lasso")] float maxLassoDistance;
    [SerializeField] GameObject lasso;
    //[SerializeField] [Tooltip("Toggle lasso pull mechanic")] bool toggleLasso;
    float currentLassoCharge;
    bool isCharging;
    bool isLassoOut;
    LineRenderer lr;
    [Header("Kick Properties")]
    [SerializeField] [Tooltip("The force of the kick")] float kickForce;
    MainControls mc;

    [Header("Sound")]
    [SerializeField] private JukeBox jukebox;

    bool kicking;
    GameObject kick;
    IsoPlayerController pc;



    private void Awake()
    {
        mc = new MainControls();
        jukebox.SetTransform(transform);
    }

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        isLassoOut = false;
        isCharging = false;
        currentLassoCharge = 0;
        kicking = false;
        kick = transform.Find("KickHitbox").gameObject;
        kick.SetActive(false);
        pc = GetComponentInParent<IsoPlayerController>();
    }

    private void OnEnable()
    {
        mc.Enable();
        mc.Main.Primary.performed += _ => Kick();
        mc.Main.Secondary.performed += _ => LassoCharge();
        mc.Main.Secondary.canceled += _ => Lasso();
    }

    private void OnDisable()
    {
        mc.Disable();
    }

    private void Kick()
    {
        if (!kicking && !isCharging && !pc.isStunned)
        {
            pc.lookAtMouse();
            kicking = true;
            pc.attackState = Helpers.KICKING;
            kick.SetActive(true);
            //Debug.Log("I kick em!");
        }

    }

   

    public void ActivateKick(GameObject target)
    {
        //Debug.Log("Apply force");
        Rigidbody rb = target.GetComponent<Rigidbody>();
        rb.velocity = transform.forward * kickForce + Vector3.up * kickForce / 2 / rb.mass;
        target.GetComponent<IKickable>().Kicked();
        
    }

    private void LassoCharge()
    {
        if (!pc.isStunned)
        {
            LassoBehavior lb = FindObjectOfType<LassoBehavior>();
            isLassoOut = (lb != null);
            if (!isLassoOut)
            {
                currentLassoCharge = 0;
                lr.enabled = true;
                pc.attackState = Helpers.CHARGING;
                isCharging = true;
            }
            else
            {
                Pull(lb);
            }
        }
    }

    private void Update()
    {
        if (isCharging && !pc.isStunned)
        {
            currentLassoCharge = Mathf.Clamp(currentLassoCharge + Time.deltaTime, 0, lassoChargeTime);
            float currentDistance = minLassoDistance + (maxLassoDistance - minLassoDistance) * currentLassoCharge / lassoChargeTime;
            Vector3[] positions = { transform.position, transform.forward * currentDistance + transform.position };
            lr.SetPositions(positions);
        }
        else
        {
            lr.enabled = false;
        }
    }

    private void Lasso()
    {
        if(!isLassoOut && !kicking && !pc.isStunned)
        { 

            isCharging = false;
            pc.attackState = Helpers.NOTATTACKING;
            GameObject temp = Instantiate(lasso, transform.position, Quaternion.identity);
            temp.GetComponent<Rigidbody>().velocity = transform.forward * lassoSpeed;
            float currentDistance = minLassoDistance + (maxLassoDistance - minLassoDistance) * currentLassoCharge / lassoChargeTime;
            temp.GetComponent<LassoBehavior>().maxDistance = currentDistance;
            
        }

    }

    private void Pull(LassoBehavior lb)
    {
        GameObject target = lb.getAttachment();
        if (target != null)
        {
            if (target.GetComponent<Rigidbody>() != null)
            {
                Rigidbody rb = target.GetComponent<Rigidbody>();
                var (success, position) = pc.GetMousePosition();
                if (success)
                {
                    Vector3 dir = ((transform.position + (position - transform.position).normalized * (Vector3.Distance(transform.position, target.transform.position) / 1.5f)) - target.transform.position).normalized;
                    dir.y = 0;
                    rb.velocity = (dir * pullForce / rb.mass) + Vector3.up / 5 * pullForce;
                }
                else
                {
                    Vector3 dir = ((transform.position + transform.forward * (Vector3.Distance(transform.position, target.transform.position) / 1.5f)) - target.transform.position).normalized;
                    dir.y = 0;
                    rb.velocity = (dir * pullForce / rb.mass) + Vector3.up / 5 * pullForce;
                }
                //target.GetComponent<IPullable>().Pulled();
            }
            target.GetComponent<IPullable>().Pulled();
        }
        Destroy(lb.gameObject);
    }


    public void KickEnd()
    {
        pc.attackState = Helpers.NOTATTACKING;
        kick.SetActive(false);
        kicking = false;
    }
}
