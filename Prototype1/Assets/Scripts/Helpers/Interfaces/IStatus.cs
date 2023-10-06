using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class IStatus : MonoBehaviour
{
    [SerializeField] protected float effectDuration;
    protected float currentTime;
    protected Coroutine timerCoroutine;


    public virtual void Activate()
    {
        Effect();
        currentTime = 0;
        if (timerCoroutine == null)
        {
            timerCoroutine = StartCoroutine(Timer());
        }
        Debug.Log("Activate Status On " + name);
    }    

    protected abstract void Deactivate();

    protected abstract void Effect();

    protected IEnumerator Timer()
    {
        while (currentTime <= effectDuration)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
        Deactivate();
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
