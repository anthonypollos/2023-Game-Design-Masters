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
    private ParticleSystem.EmissionModule em;
    private Light glow;
    private int mod = 1;
    Coroutine coroutine = null;
    
    protected override void Deactivate()
    {
        /*
        if(an != null) 
            an.SetBool("Burning", false);
        */
        effectOn = false;
        if (fireEffect != null)
        {
            fireEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            em.enabled = false;
            if (glow != null)
            {
                glow.enabled = false;
            }
            //Check if the fire particle gameobject has an AudioSource and stop it if it does.
            //This code may be messy and have extra steps than needed. Please double check it when you can, Josh. -Sean
            if (fireEffect.gameObject.GetComponent<AudioSource>() != null)
            {
                fireEffect.gameObject.GetComponent<AudioSource>().Stop();
            }
        }
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
            //Check if the fire particle gameobject has an AudioSource and play it if it does.
            //This code may be messy and have extra steps than needed. Please double check it when you can, Josh. -Sean
            if (fireEffect.gameObject.GetComponent<AudioSource>() != null)
            {
                fireEffect.gameObject.GetComponent<AudioSource>().Play();
            }
            em.enabled = true;
            if (glow != null)
            {
                glow.enabled = true;
            }
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
        fireEffect = GetComponentInChildren<ParticleSystem>(true);
        if (fireEffect != null)
        {
            //Debug.Log("got fire on" + name);
            em = fireEffect.emission;
            em.enabled = false;
            glow = fireEffect.gameObject.GetComponentInChildren<Light>(true);
            if (glow != null)
            {
                glow.enabled = false;
            }
        }
        if (startOnFire)
        {
            defaultEffectDuration = Mathf.Infinity;
            Activate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FireVulnerable(float duration)
    {
        if(coroutine!=null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = StartCoroutine(Vulnerable(duration));
    }

    IEnumerator Vulnerable(float duration)
    {
        mod = 2;
        yield return new WaitForSeconds(duration);
        mod = 1;
        coroutine = null;
    }


}
