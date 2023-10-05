using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornBehavior : MonoBehaviour, IProjectile
{

    [SerializeField] float despawnTime = 20f;
    [SerializeField] int damage = 15;
    [SerializeField] float speed = 4f;
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
        IDamageable id = hit.GetComponent<IDamageable>();
        if (id != null)
        {
            id.TakeDamage(damage);
            Destroy(gameObject);
        }
        if (gameObject.CompareTag("Wall") || gameObject.CompareTag("Ground"))
            Destroy(gameObject);
    }

    public void Shoot(Vector3 dir, Vector3 playerPos)
    {
        GetComponent<Rigidbody>().velocity = dir * speed;
    }
}
