using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowingArea : MonoBehaviour
{
    [SerializeField] [Range(0,1)] float slowingAmount = 0.5f;
    List<ISlowable> slowables;

    private void Start()
    {
        slowables = new List<ISlowable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        ISlowable slowable;
        if(other.transform.TryGetComponent<ISlowable>(out slowable))
        {
            if(!slowables.Contains(slowable))
                slowables.Add(slowable);
            slowable.EnterSlowArea(slowingAmount);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        ISlowable slowable;
        if(other.transform.TryGetComponent<ISlowable> (out slowable))
        {
            if(slowables.Contains(slowable))
                slowables.Remove(slowable);
            slowable.ExitSlowArea(slowingAmount);
        }
    }

    private void OnDestroy()
    {
        foreach (ISlowable slowable in slowables)
            slowable.ExitSlowArea(slowingAmount);
    }
}
