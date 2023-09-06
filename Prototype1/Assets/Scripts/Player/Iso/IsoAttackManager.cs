using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoAttackManager : MonoBehaviour, ICanKick
{
    [SerializeField][Tooltip("The speed of the lasso")] float lassoSpeed;
    [SerializeField][Tooltip("The force of the pull")] float pullForce;
    [SerializeField][Tooltip("The force of the kick")] float kickForce;
    [SerializeField] GameObject lasso;
    //Camera cam;
    MainControls mc;
    bool kicking;
    GameObject kick;
    IsoPlayerController pc;



    private void Awake()
    {
        mc = new MainControls();
    }

    // Start is called before the first frame update
    void Start()
    {
        kicking = false;
        kick = transform.Find("KickHitbox").gameObject;
        kick.SetActive(false);
        pc = GetComponentInParent<IsoPlayerController>();
    }

    private void OnEnable()
    {
        mc.Enable();
        mc.Main.Primary.performed += _ => Kick();
        mc.Main.Secondary.performed += _ => Lasso();
    }

    private void OnDisable()
    {
        mc.Disable();
    }

    private void Kick()
    {
        if (!kicking)
        {
            StartCoroutine(QuickKick());
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

    private void Lasso()
    {
        LassoBehavior lb = FindObjectOfType<LassoBehavior>();

        if (lb == null)
        {
            
            GameObject temp = Instantiate(lasso, transform.position, Quaternion.identity);
            temp.GetComponent<Rigidbody>().velocity = transform.forward * lassoSpeed;
            
        }
        else
        {
            GameObject target = lb.getAttachment();
            if (target != null)
            {
                if (target.GetComponent<Rigidbody>() != null)
                {
                    Rigidbody rb = target.GetComponent<Rigidbody>();
                    Vector3 dir = ((transform.position + transform.forward * (Vector3.Distance(transform.position, target.transform.position) / 1.5f)) - target.transform.position).normalized;
                    dir.y = 0;
                    rb.velocity = (dir * pullForce / rb.mass) + Vector3.up/5*pullForce;
                    //target.GetComponent<IPullable>().Pulled();
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
