using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chandelier : MonoBehaviour, IToggleable, ITrap
{
    Rigidbody rb;
    [SerializeField] int dmg = 20;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Toggle(IsoAttackManager player = null)
    {
        rb.useGravity = true;

    }

    public bool GetToggle()
    {
        return rb.useGravity;
    }

    public void ActivateTrap(GameObject target)
    {
        IDamageable temp = target.GetComponent<IDamageable>();
        if (temp != null)
        {
            int mod = 1;
            if (target.CompareTag("Player"))
                mod = 2;
            temp.TakeDamage(dmg/mod);
        }
    }
}
