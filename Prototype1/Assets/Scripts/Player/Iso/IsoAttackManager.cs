using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoAttackManager : MonoBehaviour, ICanKick
{
    [Header("Lasso properties")]
    [SerializeField] [Tooltip("The speed of the lasso")] float lassoSpeed;
    [SerializeField] [Tooltip("The speed of the pull")] float pullForce;
    [SerializeField] [Range(0f, 1f)] [Tooltip("The % force of the pull if aiming fully away from the player")] float minPullForceModifier = 0.5f;
    [SerializeField] [Tooltip("How far a pulled object will go assuming 1 mass")] float pullCarryDistance;
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
    [SerializeField] [Tooltip("How far a kicked object will go assuming 1 mass")] float kickCarryDistance;
    MainControls mc;

    [Header("Sound")]
    [SerializeField] private JukeBox jukebox;

    bool kicking;
    GameObject kick;
    [HideInInspector]
    public IsoPlayerController pc;
    GameController gc;



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
        gc = FindObjectOfType<GameController>();
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
        if (!kicking && !isCharging && !pc.moveable.isLaunched)
        {
            if (InputChecker.instance.IsController())
                pc.LookAtAim();
            else
                pc.LookAtMouse();
            kicking = true;
            pc.attackState = Helpers.ATTACKING;
            kick.SetActive(true);
            //Debug.Log("I kick em!");
        }

    }

   

    public void ActivateKick(GameObject target)
    {
        //Debug.Log("Apply force");
        Moveable moveable = target.GetComponent<Moveable>();
        if(moveable != null)
            moveable.Launched(transform.forward * kickCarryDistance ,kickForce);
        IKickable kick = target.GetComponent<IKickable>();
        if (kick != null)
            kick.Kicked();
        
    }

    private void LassoCharge()
    {
        if (!pc.moveable.isLaunched)
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
        if (isCharging && !pc.moveable.isLaunched)
        {
            lr.enabled = true;
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
        if(!isLassoOut && !kicking && !pc.moveable.isLaunched)
        { 

            isCharging = false;
            pc.attackState = Helpers.NOTATTACKING;
            GameObject temp = Instantiate(lasso, transform.position, Quaternion.identity);
            temp.GetComponent<Rigidbody>().velocity = transform.forward * lassoSpeed;
            float currentDistance = minLassoDistance + (maxLassoDistance - minLassoDistance) * currentLassoCharge / lassoChargeTime;
            LassoBehavior lb = temp.GetComponent<LassoBehavior>();
            lb.SetValues(pullCarryDistance, minPullForceModifier, maxLassoDistance, transform);
            
        }

    }

    public void Pull(LassoBehavior lb)
    {
        GameObject target;
        Moveable moveable;
        (target, moveable) = lb.GetAttachment();
        if (target != null)
        {
            if (moveable != null)
            {
                Vector3 dir;
                float calculatedDistance;
                (dir, calculatedDistance) = lb.GetValues();
                Debug.Log(calculatedDistance);
                Debug.Log(dir.magnitude);
                moveable.Launched(dir * calculatedDistance, pullForce);
                //Rigidbody rb = target.GetComponent<Rigidbody>();
                //var (success, position) = pc.GetMousePosition();
                //if (success)
                //{
                   // Vector3 dir = ((transform.position + (position - transform.position).normalized * (Vector3.Distance(transform.position, target.transform.position) / 1.5f)) - target.transform.position).normalized;
                    //dir.y = 0;
                    //rb.velocity = (dir * pullForce / rb.mass) + Vector3.up / 5 * pullForce;
                //}
               // else
                //{
                    //Vector3 dir = ((transform.position + transform.forward * (Vector3.Distance(transform.position, target.transform.position) / 1.5f)) - target.transform.position).normalized;
                    //dir.y = 0;
                    //rb.velocity = (dir * pullForce / rb.mass) + Vector3.up / 5 * pullForce;
                //}
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
