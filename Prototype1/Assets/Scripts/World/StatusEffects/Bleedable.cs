using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleedable : IStatus
{
    [SerializeField] private int damagePerTick = 4;
    [SerializeField] private float tickInterval = 0.5f;
    private Coroutine isBleeding;
    private IDamageable iDamageable;
    [HideInInspector] public Animator an;
    private ParticleSystem bleedEffect;
    private ParticleSystem.EmissionModule em;


    protected override void Deactivate()
    {

        effectOn = false;
        /*if (bleedEffect != null)
        {
            bleedEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            em.enabled = false;
            if (bleedEffect.gameObject.GetComponent<AudioSource>() != null)
            {
                bleedEffect.gameObject.GetComponent<AudioSource>().Stop();
            }
        } */
        StopCoroutine(isBleeding);
        isBleeding = null;

        //If we're an enemy and we have a bleed slider, clear the bleed slider.
        if (GetComponent<EnemyHealth>() != null && GetComponent<EnemyHealth>().GetBleedSlider() != null) GetComponent<EnemyHealth>().ClearBleedSlider();

        //throw new System.NotImplementedException();
    }

    protected override void Effect()
    {
        //Debug.Log("be on fire now");
        /*if (bleedEffect != null)
        {
            bleedEffect.Play(true);
            if (bleedEffect.gameObject.GetComponent<AudioSource>() != null)
            {
                bleedEffect.gameObject.GetComponent<AudioSource>().Play();
            }
            em.enabled = true;
        }*/
        if (an != null)
            an.SetBool("Bleeding", true);
        if (isBleeding == null)
        {
            effectOn = true;
            isBleeding = StartCoroutine(Damage());
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
                iDamageable.TakeDamage(damagePerTick,DamageTypes.BLEED);
                //Force blood to spawn specifically on enemies
                if (GetComponent<EnemyHealth>() != null) GetComponent<EnemyHealth>().ForceSpawnBlood();
            }
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        an = GetComponent<Animator>();
        iDamageable = GetComponent<IDamageable>();
        isBleeding = null;
        /*bleedEffect = GetComponentInChildren<ParticleSystem>(true);
        if (bleedEffect != null)
        {
            Debug.Log("got fire on" + name);
            em = bleedEffect.emission;
            em.enabled = false;
        }*/
    }

    // Update is called once per frame
    void Update()
    {

    }
}

