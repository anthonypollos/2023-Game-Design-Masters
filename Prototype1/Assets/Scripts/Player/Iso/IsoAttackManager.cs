using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoAttackManager : MonoBehaviour
{
    [SerializeField][Tooltip("The speed of the lasso")] float lassoSpeed;
    [SerializeField][Tooltip("The force of the pull")] float pullForce;
    //[SerializeField][Tooltip("The force of the kick")] float kickForce;
    [SerializeField] GameObject lasso;
    Camera cam;
    MainControls mc;
    //bool kicking;
    //GameObject kick;

    private void Awake()
    {
        mc = new MainControls();
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        //kicking = false;
        //kick = cam.transform.GetChild(0).gameObject;
    }

    private void OnEnable()
    {
        mc.Enable();
        //mc.Main.Primary.performed += _ => Kick();
        mc.Main.Secondary.performed += _ => Lasso();
    }

    private void Lasso()
    {
        LassoBehavior lb = FindObjectOfType<LassoBehavior>();

        if (lb != null)
        {
            Ray raycast = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(raycast, out RaycastHit hitInfo))
            {
                Vector3 hypotenuseLine = (raycast.origin = hitInfo.point).normalized;

                float angle = Vector3.Angle(hypotenuseLine, new Vector3(hypotenuseLine.x, 0, hypotenuseLine.z));
                float opposite = lb.startingPos.y - hitInfo.point.y;

                float hypotenuseLength = opposite / Mathf.Sin(angle * Mathf.Deg2Rad);

                Vector3 hitPosition = hitInfo.point + (hypotenuseLine * hypotenuseLength);
                Vector3 hitDirection = hitPosition - lb.startingPos;

                GameObject temp = Instantiate(lasso, lb.startingPos, lb.startingRot);
                temp.GetComponent<Rigidbody>().velocity = cam.transform.forward * lassoSpeed;
                raycast.direction = hitDirection.normalized;
            }
        }
        else
        {
            GameObject enemy = lb.getAttachment();
            if (enemy != null)
            {
                if (enemy.GetComponent<Rigidbody>() != null)
                {
                    Rigidbody rb = enemy.GetComponent<Rigidbody>();
                    Vector3 dir = ((cam.transform.position + cam.transform.forward * (Vector3.Distance(transform.position, enemy.transform.position) / 1.5f)) - enemy.transform.position).normalized;
                    rb.velocity = (dir * pullForce / rb.mass);
                    enemy.GetComponent<EnemyBehavior>().Pulled();
                }
                else
                {
                    IPullable pl = enemy.GetComponent<IPullable>();
                    if (pl != null)
                    {
                        pl.Pull();
                    }
                    //Activate pull effect
                }
            }
            Destroy(lb.gameObject);
        }

    }
}
