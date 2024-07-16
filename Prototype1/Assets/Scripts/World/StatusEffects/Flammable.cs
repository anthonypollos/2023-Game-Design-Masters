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
    public bool isBurning = false;
    [HideInInspector] public Animator an;
    [Tooltip("An array of game objects that you want to enable or disable upon burning.\nIf no array is specified, we'll assume the first particle system we find is a fire effect instead")]
    [SerializeField] private GameObject[] fireEffects;
    private ParticleSystem fireEffect;
    private ParticleSystem.EmissionModule em;
    private Light glow;
    private int mod = 1;
    Coroutine coroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        an = GetComponent<Animator>();
        iDamageable = GetComponent<IDamageable>();
        onFire = null;

        //If the effects are not serialized, look for a particle system gameobject that's a child of this
        if (fireEffects.Length <= 0) fireEffect = GetComponentInChildren<ParticleSystem>(true);

        //if we use the serialized array, deactivate all objects
        if (fireEffects.Length > 0)
        {
            foreach (GameObject g in fireEffects)
            {
                g.SetActive(false);
            }
        }

        //if we use the old system, get the emission and glow, then deactivate them
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

        //Finally, if we start on fire, activate
        if (startOnFire)
        {
            defaultEffectDuration = Mathf.Infinity;
            Activate();
        }
    }

    public void StopDropAndRoll()
    {
        Deactivate();
    }

    public override void Activate()
    {

        //if we use the serialized array, activate all objects
        if (fireEffects.Length > 0 && !isBurning)
        {
            foreach (GameObject g in fireEffects)
            {
                g.SetActive(true);
            }
        }

        //if we're using the old system, let's make sure that the fire Effect object is active.
        if (fireEffect != null && !isBurning) fireEffect.gameObject.SetActive(true);
        //Ditto for the glow effect
        if (glow != null && !isBurning) glow.gameObject.SetActive(true);

        Effect();
        currentTime = 0;
        adjustedEffectDuration = 0;
        if (timerCoroutine == null)
        {
            timerCoroutine = StartCoroutine(Timer());
        }
        //Debug.Log("Activate Status On " + name);
    }

    protected override void Deactivate()
    {
        effectOn = false;
        isBurning = false;

        //if we use the serialized array, deactivate all objects
        if (fireEffects.Length > 0)
        {
            foreach (GameObject g in fireEffects)
            {
                g.SetActive(false);
            }
        }

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

        //If we're an enemy and we have a fire slider, clear the fire slider.
        if (GetComponent<EnemyHealth>() != null && GetComponent<EnemyHealth>().GetFireSlider() != null) GetComponent<EnemyHealth>().ClearFireSlider();
    }

    protected override void Effect()
    {
        if(fireEffect != null && !isBurning)
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
        isBurning = true;
        while (effectOn)
        {
            yield return new WaitForSeconds(tickInterval);
            //Debug.Log("Tick Damage");
            if (iDamageable != null)
            {
                iDamageable.TakeDamage(damagePerTick, DamageTypes.FIRE);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        Flammable flammable = collision.gameObject.GetComponent<Flammable>();
        if (flammable != null && onFire != null)
        {
            Debug.Log("Collision Fire");
            flammable.Activate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Flammable flammable = other.gameObject.GetComponent<Flammable>();
        if (flammable != null && onFire != null)
        {
            flammable.Activate();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        
        Flammable flammable = collision.gameObject.GetComponent<Flammable>();
        if (flammable != null && onFire!=null)
        {
            //Debug.Log("Collision Fire");
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
