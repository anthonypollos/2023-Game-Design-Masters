using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : MonoBehaviour
{
    private Moveable moveable;
    [SerializeField] GameObject explosion;
    private int health;
    [SerializeField] float fuse = 5f;
    [SerializeField] JukeBox jukebox;

    // Start is called before the first frame update
    void Start()
    {
        health = 10;
        moveable = GetComponent<Moveable>();
        
    }

   

    public void TakeDamage(int dmg)
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
        jukebox.PlaySound(0);
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
