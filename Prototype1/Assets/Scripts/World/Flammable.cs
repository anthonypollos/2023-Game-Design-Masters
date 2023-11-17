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
    [HideInInspector] public Animator an;
    private ParticleSystem fireEffect;

    
    protected override void Deactivate()
    {
        /*
        if(an != null) 
            an.SetBool("Burning", false);
        */
        effectOn = false;
        StopCoroutine(onFire);
        onFire = null;
        //throw new System.NotImplementedException();
    }

    protected override void Effect()
    {
        //Debug.Log("be on fire now");
        if(fireEffect != null)
        {
            fireEffect.Play(true);
        }
        if(an != null)
            an.SetBool("Burning", true);
        if (onFire == null)
        {
            effectOn = true;
            onFire = StartCoroutine(Damage());
        }
    }

    protected IEnumerator Damage()
    {
        while (effectOn)
        {
            yield return new WaitForSeconds(tickInterval);
            //Debug.Log("Tick Damage");
            if (iDamageable != null)
            {
                iDamageable.TakeDamage(damagePerTick);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Flammable flammable = collision.gameObject.GetComponent<Flammable>();
        if (flammable != null && onFire!=null)
        {
            flammable.Activate();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Flammable flammable = other.gameObject.GetComponent<Flammable>();
        if (flammable != null && onFire!=null)
        {
            flammable.Activate();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        an = GetComponent<Animator>();
        iDamageable = GetComponent<IDamageable>();
        onFire = null;
        fireEffect = GetComponentInChildren<ParticleSystem>();
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
