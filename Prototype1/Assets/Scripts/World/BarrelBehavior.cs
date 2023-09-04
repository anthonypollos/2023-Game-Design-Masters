using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelBehavior : MonoBehaviour, IKickable, IPullable, IDamageable
{
    bool primed;
    [SerializeField] GameObject explosion;
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

    }

    public void Lassoed()
    {

    }

    public void TakeDamage(int dmg)
    {
        Explode();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(primed)
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
