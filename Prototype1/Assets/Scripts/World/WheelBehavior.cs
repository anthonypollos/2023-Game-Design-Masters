using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelBehavior : MonoBehaviour, IKickable, IPullable, IDamageable
{
    bool primed;
    private Moveable moveable;
    private int health;
    [SerializeField] int dmg = 20;

    // Start is called before the first frame update
    void Start()
    {
        health = 10;
        moveable = GetComponent<Moveable>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Kicked()
    {
        
    }

    public void Pulled()
    {
    }

    public void Lassoed()
    {

    }
}
