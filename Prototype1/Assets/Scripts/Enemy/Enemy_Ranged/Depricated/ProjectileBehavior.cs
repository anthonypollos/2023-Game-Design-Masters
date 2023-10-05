using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour, IProjectile
{

    [SerializeField] float despawnTime = 10f;
    [SerializeField] int damage = 15;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Despawn());
    }

    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("hit");
        GameObject hit = other.gameObject;
        if (gameObject.CompareTag("Wall") || gameObject.CompareTag("Ground"))
            Destroy(gameObject);
        IDamageable id = hit.GetComponent<IDamageable>();
        if(id != null)
        {
            id.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    public void Shoot(Vector3 dir, Vector3 playerPos)
    {
        ;
    }
}
