using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrapBehavior : MonoBehaviour
{
    [SerializeField] int dmg = 20;

    [SerializeField] private JukeBox jukebox;

    private void Awake()
    {
        jukebox.SetTransform(transform);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        IDamageable temp = collision.gameObject.GetComponent<IDamageable>();
        if (temp != null)
        {
            temp.TakeDamage(dmg);
            collision.gameObject.GetComponent<EnemyInteractionBehaviorTemplate>().stunned = true;
        }
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        IDamageable temp = other.gameObject.GetComponent<IDamageable>();
        if (temp != null)
        {
            other.gameObject.GetComponent<EnemyInteractionBehaviorTemplate>().Stun(5);
            temp.TakeDamage(dmg);
            jukebox.PlaySound(0);
        }
    }

    
}
