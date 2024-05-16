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
        ISlowable[] slowItems = null;
        slowItems = other.GetComponents<ISlowable>();
        if(slowItems.Length>0)
        {
            foreach (ISlowable slowable in slowItems)
            {
                if (!slowables.Contains(slowable))
                    slowables.Add(slowable);
                slowable.EnterSlowArea(slowingAmount);
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        ISlowable[] slowItems = null;
        slowItems = other.GetComponents<ISlowable>();
        if (slowItems.Length>0)
        {
            foreach (ISlowable slowable in slowItems)
            {
                if (slowables.Contains(slowable))
                    slowables.Remove(slowable);
                slowable.ExitSlowArea(slowingAmount);
            }
        }
    }

    private void OnDestroy()
    {
        foreach (ISlowable slowable in slowables)
            slowable.ExitSlowArea(slowingAmount);
    }
}
