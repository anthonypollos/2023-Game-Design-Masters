using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoBehavior : MonoBehaviour
{
    [SerializeField] float maxDistance;
    private GameObject attached;
    private bool grounded;
    public Vector3 startingPos;
    public Quaternion startingRot;
    // Start is called before the first frame update
    private void Start()
    {
        grounded = false;
        attached = null;
        startingPos = transform.position;
        startingRot = transform.rotation;
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject temp = collision.gameObject;
        if (attached == null && !grounded)
        {
            if (temp.CompareTag("Ground"))
            {
                grounded = true;
                gameObject.GetComponent<Rigidbody>().useGravity = true;
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            else if (temp.GetComponent<IPullable>() != null)
            {
                attached = temp;
                attached.GetComponent<IPullable>().Lassoed();
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
    }

    public GameObject getAttachment()
    {
        return attached;
    }
}
