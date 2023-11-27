using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chandelier : MonoBehaviour, IToggleable, ITrap
{
    Rigidbody rb;
    private Moveable moveable;
    [SerializeField] int dmg = 20;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        moveable = GetComponent<Moveable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Toggle()
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
            temp.TakeDamage(dmg);
        }
    }
}
