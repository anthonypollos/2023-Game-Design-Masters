using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class IStatus : MonoBehaviour
{
    [SerializeField] protected float defaultEffectDuration;
    protected float adjustedEffectDuration = 0;
    protected float currentTime;
    protected Coroutine timerCoroutine;
    protected bool effectOn = false;


    public virtual void Activate()
    {
        Effect();
        currentTime = 0;
        adjustedEffectDuration = 0;
        if (timerCoroutine == null)
        {
            timerCoroutine = StartCoroutine(Timer());
        }
        //Debug.Log("Activate Status On " + name);
    }    

    public virtual void Activate(float time)
    {
        Effect();
        currentTime = 0;
        adjustedEffectDuration = time;
        if (timerCoroutine == null)
        {
            timerCoroutine = StartCoroutine(Timer());
        }
    }

    protected abstract void Deactivate();

    protected abstract void Effect();

    protected IEnumerator Timer()
    {
        if (adjustedEffectDuration <= 0)
        {
            while (currentTime <= defaultEffectDuration)
            {
                currentTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (currentTime <= adjustedEffectDuration)
            {
                currentTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        Deactivate();
        timerCoroutine = null;
    }
    
    public float getDefaultEffectDuration()
    {
        return defaultEffectDuration;
    }

    public float getCurrentTime()
    {
        return currentTime;
    }

    public float getAdjustedEffectDuration()
    {
        return adjustedEffectDuration;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
