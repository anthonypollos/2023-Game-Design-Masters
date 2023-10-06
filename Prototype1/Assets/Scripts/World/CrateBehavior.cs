using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateBehavior : MonoBehaviour, IKickable, IPullable, IDamageable
{
    [SerializeField] private int health;
    [SerializeField] JukeBox jukebox;
    private Moveable moveable;

    // Start is called before the first frame update
    void Start()
    {
        health = 20;
        jukebox.SetTransform(transform);
        moveable = GetComponent<Moveable>();
    }

    public void Kicked()
    {
        
    }

    public void Lassoed()
    {
       
    }

    public void Pulled()
    {
        
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Break();
        }
    }

    private void Break()
    {
        Destroy(gameObject);
    }
}
