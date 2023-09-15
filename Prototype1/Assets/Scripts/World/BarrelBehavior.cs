using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelBehavior : MonoBehaviour, IKickable, IPullable, IDamageable
{
    bool primed;
    [SerializeField] GameObject explosion;
    [SerializeField] float fuse = 5f;
    // Start is called before the first frame update
    void Start()
    {
        primed = false;
    }

    public void Kicked()
    {
        primed = true;
    }

    public void Pulled()
    {
        primed = true;
    }

    public void Lassoed()
    {

    }

    public void TakeDamage(int dmg)
    {
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(fuse);
        Explode();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(primed && !collision.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
