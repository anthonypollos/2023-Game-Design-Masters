using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IsoAttackManager : MonoBehaviour, ICanKick
{
    [Header("Lasso properties")]
    [SerializeField] [Tooltip("The speed of the lasso")] float lassoSpeed;
    [SerializeField] [Tooltip("The speed of the pull")] float pullSpeed;
    [SerializeField] [Range(0f, 1f)] [Tooltip("The % force of the pull if aiming fully away from the player")] float minPullForceModifier = 0.5f;
    [SerializeField] [Tooltip("How far a pulled object will go assuming 1 mass")] float pullCarryDistance;
    [SerializeField] [Tooltip("The time it takes for full charge")] float lassoChargeTime;
    [SerializeField] [Tooltip("Minimum distance throwing the lasso")] float minThrowLassoDistance;
    [SerializeField] [Tooltip("Maximum distance throwing the lasso")] float maxThrowLassoDistance;
    [SerializeField] [Tooltip("Maximum distance of lasso till it breaks")] float maxLassoDistance;
    [SerializeField] Slider lassoRangeUIIndicator;
    [SerializeField] GameObject lasso;
    [SerializeField] GameObject lassoOrigin;
    [SerializeField] float retractionSpeed;
    [SerializeField] GameObject tendril;
    Rigidbody lassoRB;
    bool isRetracting;
    LassoBehavior lb;
    //[SerializeField] [Tooltip("Toggle lasso pull mechanic")] bool toggleLasso;
    float currentLassoCharge;
    bool isCharging;
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
    Coroutine returnCall;


    private void Awake()
    {
        mc = new MainControls();
        jukebox.SetTransform(transform);
    }

    // Start is called before the first frame update
    void Start()
    {
        isRetracting = false;
        lr = GetComponent<LineRenderer>();
        isCharging = false;
        currentLassoCharge = 0;
        kicking = false;
        kick = transform.Find("KickHitbox").gameObject;
        kick.SetActive(false);
        pc = GetComponentInParent<IsoPlayerController>();
        gc = FindObjectOfType<GameController>();
        lb = lasso.GetComponentInChildren<LassoBehavior>();
        lasso.SetActive(false);
        lb.enabled = false;
        tendril.SetActive(false);
        lassoRB = lasso.GetComponent<Rigidbody>();
        lb.SetValues(pullCarryDistance, minPullForceModifier, maxThrowLassoDistance, maxLassoDistance, lassoRangeUIIndicator);
    }

    private void OnEnable()
    {
        mc.Enable();
        mc.Main.Primary.performed += _ => Kick();
        mc.Main.Secondary.performed += _ => LassoCharge();
        mc.Main.Secondary.canceled += _ => Lasso();
        mc.Main.Release.performed += _ => Release();
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
            jukebox.PlaySound(0);
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
            if (!lasso.activeInHierarchy)
            {
                currentLassoCharge = 0;
                lr.enabled = true;
                pc.attackState = Helpers.LASSOING;
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
            float currentDistance = minThrowLassoDistance + (maxThrowLassoDistance - minThrowLassoDistance) * currentLassoCharge / lassoChargeTime;
            Vector3[] positions = { transform.position, transform.forward * currentDistance + transform.position };
            lr.SetPositions(positions);
        }
        else
        {
            lr.enabled = false;
        }

        if(isRetracting)
        {
            lassoRB.velocity = (lassoOrigin.transform.position - lasso.transform.position).normalized * lassoSpeed;
            if (Vector3.Distance(lassoOrigin.transform.position, lasso.transform.position) < 1f)
                Retracted();
        }
    }

    private void Lasso()
    {
        if(!lasso.activeInHierarchy && !kicking && !pc.moveable.isLaunched && isCharging)
        { 

            isCharging = false;
            pc.attackState = Helpers.LASSOED;
            //GameObject temp = Instantiate(lasso, transform.position, Quaternion.identity);
            lasso.SetActive(true);
            tendril.SetActive(true);
            lb.enabled = true;
            lasso.transform.parent = null;
            lassoRB.velocity = transform.forward * lassoSpeed;
            float currentDistance = minThrowLassoDistance + (maxThrowLassoDistance - minThrowLassoDistance) * currentLassoCharge / lassoChargeTime;
            //LassoBehavior lb = temp.GetComponent<LassoBehavior>();
            lb.SetValues(pullCarryDistance, minPullForceModifier, currentDistance, maxLassoDistance, lassoRangeUIIndicator);
        }

    }

    public void Release()
    {
        if(lasso.activeInHierarchy && returnCall == null)
        {
            lassoRangeUIIndicator.gameObject.SetActive(false);
            returnCall = StartCoroutine(WaitForRetraction());
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
                //Debug.Log(calculatedDistance);
                //Debug.Log(dir.magnitude);
                moveable.Launched(dir * calculatedDistance, pullSpeed);
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
            lassoRangeUIIndicator.gameObject.SetActive(false);
            lb.transform.parent = null;
            target.GetComponent<IPullable>().Pulled();
        }
        Release();
    }


    public void KickEnd()
    {
        if (lasso.activeInHierarchy)
            pc.attackState = Helpers.LASSOED;
        else
            pc.attackState = Helpers.NOTATTACKING;
        kick.SetActive(false);
        kicking = false;
    }

    IEnumerator WaitForRetraction()
    {
        //yield return new WaitForSeconds(0.15f);
        yield return new WaitUntil(lb.TriggerRelease);
        Retraction();
        returnCall = null;
    }

    void Retraction()
    {
        lassoRB.isKinematic = false;
        lb.enabled = false;
        isRetracting = true;
        lasso.transform.parent = null;
    }

    void Retracted()
    {
        isRetracting = false;
        lasso.transform.parent = transform;
        lasso.transform.localPosition = lassoOrigin.transform.localPosition;
        lasso.SetActive(false);
        tendril.SetActive(false);
        if(pc.attackState!=Helpers.ATTACKING)
            pc.attackState = Helpers.NOTATTACKING;
    }

    
}
