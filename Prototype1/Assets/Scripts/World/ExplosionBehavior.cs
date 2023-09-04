using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBehavior : MonoBehaviour
{
    [SerializeField] int dmg;
    [SerializeField] float explosiveForce;
    [SerializeField] LayerMask layerMask;
    private void Start()
    {
        StartCoroutine(DestroySelf());
    }
    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject entity = other.gameObject;
        bool visible = CanSee(other.transform);

        if (visible)
        {
            IDamageable damaged = entity.GetComponent<IDamageable>();
            if (damaged != null) damaged.TakeDamage(dmg);
            IKickable kicked = entity.GetComponent<IKickable>();
            if (kicked != null) kicked.Kicked();
            Rigidbody rb = entity.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (entity.transform.position - transform.position);
                dir.y = 0;

                rb.velocity = (dir.normalized + Vector3.up / 5) * explosiveForce;

            }
        }
    }

    private bool CanSee(Transform target)
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, target.position - transform.position, Color.red);
        if (Physics.Raycast(transform.position, target.position - transform.position, out hit, Mathf.Infinity, layerMask))
        {
            //Debug.Log(hit.transform.name);
            if (hit.transform == target)
            {
                return true;
            }
            else
            {
                //Debug.Log(hit.transform.name);
                return false;
            }
        }
        //Debug.Log("no hit");
        return false;
    }
}
