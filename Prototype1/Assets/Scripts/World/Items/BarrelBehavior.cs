using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelBehavior : MonoBehaviour, IKickable, IPullable, IDamageable
{
    bool primed;
    private Moveable moveable;
    [SerializeField] GameObject explosion;
    private int health;
    [SerializeField] float fuse = 5f;
    [SerializeField] JukeBox jukebox;

    bool wasOn = false;
    [SerializeField] bool respawning = false;
    [SerializeField] float respawnDelay = 5f;
    Vector3 initialPos;
    Quaternion initialRot;

    private void OnEnable()
    {
        wasOn = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
        initialRot = transform.rotation;
        primed = false;
        health = 10;
        moveable = GetComponent<Moveable>();
        primed = false;
        jukebox.SetTransform(transform);
    }

    public void Kicked()
    {
        primed = true;
    }

    public void Pulled(IsoAttackManager player = null)
    {
    }

    public void Lassoed()
    {

    }

    public void Break()
    {
        
    }

    public void TakeDamage(int dmg, DamageTypes damageType = DamageTypes.BLUGEONING)
    {
        health -= dmg;
        if (health <= 0)
        {
            Explode();
        }
        else
        {
            StartCoroutine(Timer());
        }
    }

    public bool WillBreak(int dmg)
    {
        return (dmg >= health);
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(fuse);
        Explode();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player") && moveable.isLaunched)
        {
            if (!collision.gameObject.CompareTag("Ground"))
            { 
                Explode();
            }
        }

    }

    private void Explode()
    {
        if (!primed)
        {
            primed = true;
            jukebox.PlaySound(0);
            Instantiate(explosion, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (wasOn)
        {
            wasOn = false;
            if (respawning)
            {
                GameController.instance.Respawn(gameObject, respawnDelay, initialPos, initialRot);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
