using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flammable : IStatus
{
    [SerializeField] private int damagePerTick = 4;
    [SerializeField] private float tickInterval = 0.5f;
    private Coroutine onFire;
    private IDamageable iDamageable;
    [SerializeField] bool startOnFire;
    
    protected override void Deactivate()
    {
        throw new System.NotImplementedException();
    }

    protected override void Effect()
    {
        onFire = StartCoroutine(Damage());
    }

    protected IEnumerator Damage()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickInterval);
            Debug.Log("Tick Damage");
            if (iDamageable != null)
            {
                iDamageable.TakeDamage(damagePerTick);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Flammable flammable = collision.gameObject.GetComponent<Flammable>();
        if (flammable != null)
        {
            flammable.Activate();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Flammable flammable = other.gameObject.GetComponent<Flammable>();
        if (flammable != null)
        {
            flammable.Activate();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        iDamageable = GetComponent<IDamageable>();
        if (startOnFire)
        {
            effectDuration = Mathf.Infinity;
            Activate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
