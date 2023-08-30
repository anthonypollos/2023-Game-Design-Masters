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
    bool kicking;
    GameObject kick;

    private void Awake()
    {
        mc = new MainControls();
    }
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        kicking = false;
        kick = cam.transform.GetChild(0).gameObject;
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
            Debug.Log("I kick em!");
        }

    }

    public void activateKick(GameObject enemy)
    {
        Debug.Log("Apply force");
        enemy.GetComponent<Rigidbody>().velocity = transform.forward * kickForce + Vector3.up * kickForce/2;
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
            GameObject enemy = lb.getAttachment();
            if(enemy != null)
            {
                Rigidbody rb = enemy.GetComponent<Rigidbody>();
                Vector3 dir = ((cam.transform.position + cam.transform.forward * (Vector3.Distance(transform.position, enemy.transform.position) / 2)) - enemy.transform.position).normalized;
                rb.AddForce(dir * pullForce, ForceMode.Impulse);
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
