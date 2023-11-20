using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBehavior : MonoBehaviour
{
    [SerializeField] int dmg = 20;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamageable temp = collision.gameObject.GetComponent<IDamageable>();
        if (temp != null)
        {
            temp.TakeDamage(dmg);
        }
    }
}
