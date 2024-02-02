using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IsoAttackManager : MonoBehaviour, ICanKick
{
    [Header("Lasso properties")]
    [SerializeField] [Tooltip("The speed of the lasso")] float lassoSpeed;
    [SerializeField] [Tooltip("The speed of the pull")] float minPullSpeed = 9f;
    [SerializeField] [Tooltip("The max speed of the pull")] float maxPullSpeed = 24f;
    [SerializeField] [Tooltip("Toss speed")] float tossSpeed = 9f;
    [SerializeField] [Tooltip("Time it takes to reach max speed")] float timeToMax = 1.5f;
    [SerializeField] [Tooltip("Time it takes holding to reset speed")] float timeToReset = 0.5f;
    float currentTime = 0;
    float restTime = 0;
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
    [SerializeField] Image sliderFill;
    Rigidbody lassoRB;
    bool isRetracting;
    bool pulling;
    [HideInInspector] public LassoBehavior lb { get; private set; }
    //[SerializeField] [Tooltip("Toggle lasso pull mechanic")] bool toggleLasso;
    float currentLassoCharge;
    bool isCharging;
    LineRenderer lr;
    [Header("Kick Properties")]
    [SerializeField] [Tooltip("The force of the kick")] float kickForce;
    [SerializeField] [Tooltip("How far a kicked object will go assuming 1 mass")] float kickCarryDistance;
    [SerializeField] float kickCD;
    float trueKickCD = 0f;
    [SerializeField] Image kickCDIndicator;
    private bool canKick;
    MainControls mc;

    [Header("Sound")]
    [SerializeField] private JukeBox jukebox;

    bool kicking;
    GameObject kick;
    [HideInInspector]
    public IsoPlayerController pc;
    GameController gc;
    Coroutine returnCall;

    [Header("Animator Variables")]
    [SerializeField] Animator anim; //assigned in inspector for now; can change

    [Header("TEMP Outlines")]
    [SerializeField] OutlineToggle outlineToggle;

    private void Awake()
    {
        mc = new MainControls();
        jukebox.SetTransform(transform);
    }

    // Start is called before the first frame update
    void Start()
    {
        
        pulling = false;
        isRetracting = false;
        canKick = true;
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
        lb.SetValues(maxThrowLassoDistance, maxLassoDistance, lassoRangeUIIndicator, sliderFill);
    }

    private void OnEnable()
    {
        mc.Enable();
        mc.Main.Secondary.performed += _ => Kick();
        mc.Main.Primary.performed += _ => LassoCharge();
        mc.Main.Primary.canceled += _ => Lasso();
        mc.Main.Release.performed += _ => ForceRelease();
    }

    private void OnDisable()
    {
        mc.Disable();
    }

    private void Kick()
    {
        if (Time.timeScale != 0 && !pc.isStunned)
        {
            if (canKick && !kicking && !isCharging && !pc.moveable.isLaunched)
            {
                pc.attackState = Helpers.ATTACKING;
                if (InputChecker.instance.IsController())
                    pc.LookAtAim();
                else
                    pc.LookAtMouse();
                kicking = true;
                canKick = false;
                
                kick.SetActive(true);
                jukebox.PlaySound(0);
                //Debug.Log("I kick em!");
                // could move kick anim trigger here?
            }
        }

    }

   

    public void ActivateKick(GameObject target)
    {
        Debug.Log("Apply force");
        trueKickCD = kickCD;
        Moveable moveable = target.GetComponent<Moveable>();
        if(moveable != null)
            moveable.Launched(transform.forward * kickCarryDistance ,kickForce);
        IKickable kick = target.GetComponent<IKickable>();
        if (kick != null)
            kick.Kicked();
        
    }

    private void LassoCharge()
    {
        if (Time.timeScale != 0 && !pc.isStunned)
        {
            if (!pc.moveable.isLaunched)
            {
                //LassoBehavior lb = FindObjectOfType<LassoBehavior>();
                if (!lasso.activeInHierarchy)
                {
                    currentLassoCharge = 0;
                    lr.enabled = true;
                    pc.attackState = Helpers.LASSOING;
                    isCharging = true;

                    if (outlineToggle != null)
                        outlineToggle.ToggleOutline(true);
                }
                else
                {
                    pulling = true;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (Time.timeScale != 0 && !pc.isStunned)
        {
            if (isRetracting)
            {
                pulling = false;
                if (lassoRB.isKinematic) { lassoRB.isKinematic = false; }
                lassoRB.velocity = (lassoOrigin.transform.position - lasso.transform.position).normalized * lassoSpeed;
                if (Vector3.Distance(lassoOrigin.transform.position, lasso.transform.position) <= lassoSpeed * Time.fixedDeltaTime)
                {
                    //Debug.Log(Time.fixedDeltaTime * lassoSpeed);
                    Retracted();
                    lassoRB.isKinematic = true;
                }
            }
            else
            {
                if (pulling)
                {
                    pc.attackState = Helpers.PULLING;
                    Pull(lb);
                }
            }
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
        }
    }

    private void Update()
    {
        if (Time.timeScale != 0)
        {
            
        }

        
    }

    private void Lasso()
    {

        if (Time.timeScale != 0 && !pc.isStunned && !pc.isDead)
        {
            currentTime = 0;
            restTime = 0;
            if (!lasso.activeInHierarchy && !kicking && !pc.moveable.isLaunched && isCharging)
            {

                isCharging = false;
                pc.attackState = Helpers.LASSOED;
                //GameObject temp = Instantiate(lasso, transform.position, Quaternion.identity);
                lasso.SetActive(true);
                tendril.SetActive(true);
                lb.enabled = true;
                lasso.transform.parent = null;
                lassoRB.isKinematic = false;
                lassoRB.velocity = transform.forward * lassoSpeed;
                float currentDistance = minThrowLassoDistance + (maxThrowLassoDistance - minThrowLassoDistance) * currentLassoCharge / lassoChargeTime;
                //LassoBehavior lb = temp.GetComponent<LassoBehavior>();
                lb.SetValues(currentDistance, maxLassoDistance, lassoRangeUIIndicator, sliderFill);
                lb.Launched();

                anim.SetTrigger("TendrilThrow");
            }

            if (lasso.activeInHierarchy && lb.GetAttachment().Item2 != null)
            {
                bool flick = pulling;
                Moveable moveable = lb.GetAttachment().Item2;
                ForceRelease();
                if (flick)
                {
                    moveable.Tossed(tossSpeed);
                }
                jukebox.PlaySound(Random.Range(1,4));
            }
        }


    }

    public void ForceRelease()
    {
        if (Time.timeScale != 0)
        {
            if (lasso.activeInHierarchy)
            {
                pc.attackState = Helpers.LASSOED;
                pulling = false;
                Retraction();
            }
        }
    }

    public void Release()
    {
        if(lasso.activeInHierarchy && returnCall == null)
        {
            //lassoRangeUIIndicator.gameObject.SetActive(false);
            returnCall = StartCoroutine(WaitForRetraction());
        }
    }

    private float CalculatePullSpeed()
    {
        return Mathf.Lerp(minPullSpeed, maxPullSpeed, currentTime / timeToMax);
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
                if (dir != Vector3.zero)
                {
                    //Debug.Log(calculatedDistance);
                    //Debug.Log(dir.magnitude);
                    moveable.Launched(dir * calculatedDistance, CalculatePullSpeed());
                    currentTime = Mathf.Clamp(currentTime + Time.fixedDeltaTime, 0, timeToMax);
                    restTime = 0;
                }
                else
                {
                    moveable.Hold();
                    if (restTime >= timeToReset)
                    {
                        currentTime = 0f;
                    }
                    else
                        restTime += Time.fixedDeltaTime;
                }
                //lb.transform.position = moveable.transform.position;
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
            //lassoRangeUIIndicator.gameObject.SetActive(false);
            //lb.transform.parent = null;
            target.GetComponent<IPullable>().Pulled();
        }
        if(moveable==null)
        {
            ForceRelease();
        }
        //Release();
    }


    public void KickEnd()
    {
        if (lasso.activeInHierarchy)
            pc.attackState = Helpers.LASSOED;
        else
            pc.attackState = Helpers.NOTATTACKING;
        kick.SetActive(false);
        kicking = false;
        StartCoroutine(KickCD());
    }

    private IEnumerator KickCD()
    {

        for (float i = 0; i < trueKickCD; i += 0.01f)
        {
            yield return new WaitForSeconds(0.01f);
            kickCDIndicator.fillAmount = i / trueKickCD;
        }
        canKick = true;
        trueKickCD = 0;
        kickCDIndicator.fillAmount = 1;
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
        if (!isRetracting)
        {
            isRetracting = true;
            //Debug.Log("Start retracting");
            lb.StartRetracting();
            //lassoRB.isKinematic = false;
            //lasso.transform.parent = null;
        }
    }

    void Retracted()
    {
        isRetracting = false;
        lb.Retracted();
        lasso.transform.parent = transform;
        lasso.transform.localPosition = lassoOrigin.transform.localPosition;
        lasso.SetActive(false);
        tendril.SetActive(false);
        if(pc.attackState!=Helpers.ATTACKING)
            pc.attackState = Helpers.NOTATTACKING;

        anim.SetTrigger("NextState");
        if(outlineToggle != null)
            outlineToggle.ToggleOutline(false);
    }

    
}
