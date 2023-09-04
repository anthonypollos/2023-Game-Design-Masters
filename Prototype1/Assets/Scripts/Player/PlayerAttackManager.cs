using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
    [SerializeField] [Tooltip("The speed of the lasso")] float lassoSpeed;
    [SerializeField] [Tooltip("The force of the pull")] float pullForce;
    [SerializeField] [Tooltip("The force of the kick")] float kickForce;
    Camera cam;
    MainControls mc;
    [SerializeField] GameObject lasso;
    [SerializeField] [Tooltip("Toggle lasso pull mechanic")] bool toggleLasso;
    bool kicking;
    GameObject kick;
    PlayerController pc;

    private void Awake()
    {
        mc = new MainControls();
    }
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        kicking = false;
        kick = transform.Find("KickHitbox").gameObject;
        kick.SetActive(false);
        pc = GetComponentInParent<PlayerController>();
    }

    private void OnEnable()
    {
        mc.Enable();
        mc.Main.Primary.performed += _ => Kick();
        mc.Main.Secondary.performed += _ => Lasso();
        
    }

    private void Kick()
    {
        if(!kicking)
        {
            StartCoroutine(QuickKick());
            //Debug.Log("I kick em!");
        }

    }

    public void activateKick(GameObject target)
    {
        //Debug.Log("Apply force");
        Rigidbody rb = target.GetComponent<Rigidbody>();
        rb.velocity = transform.forward * kickForce + Vector3.up * kickForce/2/rb.mass;
        target.GetComponent<IKickable>().Kicked();
    }


    private void Lasso()
    {
        LassoBehavior lb = FindObjectOfType<LassoBehavior>();
        if (lb == null)
        {
            GameObject temp = Instantiate(lasso, transform.position + cam.transform.forward, cam.transform.localRotation);
            temp.GetComponent<Rigidbody>().velocity = cam.transform.forward * lassoSpeed;
        }
        else
        {
            GameObject target = lb.getAttachment();
            if (target != null)
            {
                if (target.GetComponent<Rigidbody>() != null)
                {
                    Rigidbody rb = target.GetComponent<Rigidbody>();
                    if (!toggleLasso)
                    { //old pull code
                        Vector3 dir = ((cam.transform.position + cam.transform.forward * (Vector3.Distance(transform.position, target.transform.position) / 1.5f)) - target.transform.position).normalized;
                        rb.velocity = (dir * pullForce / rb.mass);
                        
                    }
                    else
                    { //new pull code
                        Vector3 temp = pc.GetMovement();
                        float pullModifier;
                        if(temp.z == 0)
                        {
                            pullModifier = 1f;
                        }
                        else if(temp.z<0)
                        {
                            pullModifier = 1.25f;
                        }
                        else
                        {
                            pullModifier = .5f;
                        }
                        float aPullForce = pullForce * pullModifier;

                        Vector3 targetPos;
                        if (temp.x == 0)
                        {
                            targetPos = gameObject.transform.position;
                        }
                        else if (temp.x < 0)
                        {
                            targetPos = gameObject.transform.position + cam.transform.right * -4;
                        }
                        else
                        {
                            targetPos = gameObject.transform.position + cam.transform.right * 4;
                        }

                        Vector3 dir = (targetPos - target.transform.position).normalized;
                        rb.velocity = new Vector3(dir.x, 0, dir.z) * aPullForce + Vector3.up/5 * pullForce;
                    }
                    

                }
                target.GetComponent<IPullable>().Pulled();
            }
            Destroy(lb.gameObject);
        }
    }

    IEnumerator QuickKick()
    {
        kicking = true;
        kick.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        kick.SetActive(false);
        kicking = false;
    }




}
