using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowingArea : MonoBehaviour
{
    [SerializeField] [Range(0,1)] float slowingAmount = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        ISlowable slowable;
        if(other.transform.TryGetComponent<ISlowable>(out slowable))
        {
            slowable.EnterSlowArea(slowingAmount);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        ISlowable slowable;
        if(other.transform.TryGetComponent<ISlowable> (out slowable))
        {
            slowable.ExitSlowArea(slowingAmount);
        }
    }
}
