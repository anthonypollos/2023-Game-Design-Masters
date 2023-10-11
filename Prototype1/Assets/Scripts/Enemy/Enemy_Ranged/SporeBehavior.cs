using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SporeBehavior : MonoBehaviour, IProjectile
{
    [SerializeField]
    [Tooltip("The prefab that holds the poison pool")]
    GameObject spore;
    [SerializeField]
    [Tooltip("The prefab that shows where the projectile is going to land")]
    GameObject targetIndicator;
    [SerializeField]
    [Tooltip("The hang time of the projectile")]
    float airTime = 2.0f;
    [SerializeField]
    [Tooltip("Direct damage")]
    int damage = 5;
    Rigidbody rb;
    GameObject preview;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(Vector3 dir, Vector3 playerPos)
    {
        Vector3 temp = transform.position + dir * Vector3.Distance(playerPos, transform.position);
        RaycastHit hit;
        string[] layers = new string[] { "Ground", "Ground_Transparent" };
        Physics.Raycast(temp, Vector2.down, out hit, Mathf.Infinity, LayerMask.GetMask(layers));
        preview = Instantiate(
            targetIndicator,
            hit.point + Vector3.up * 0.01f,
            Quaternion.identity);
        preview.GetComponentInChildren<TargetPreview>().Begin(airTime);

        transform.forward = dir;
        Vector2 shootVelocity = BasicMath.ProjectileCalc(transform.position, playerPos, airTime);
        Vector3 velocity = new Vector3(dir.x * shootVelocity.x, shootVelocity.y, dir.z * shootVelocity.x);
        rb.velocity = velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damage = other.GetComponent<IDamageable>();
        if (damage != null)
            damage.TakeDamage(this.damage);
        Instantiate(spore, transform.position, Quaternion.identity);
        Destroy(preview);
        Destroy(gameObject);
    }
}
