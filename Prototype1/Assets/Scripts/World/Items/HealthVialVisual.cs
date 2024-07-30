using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthVialVisual : MonoBehaviour, IPullable
{

    private Animator animator;

    public void Break()
    {
        print("Break");
        animator.SetTrigger("Start");
    }

    public void Lassoed()
    {
        print("Lassoed");
        animator.SetTrigger("Stop");
    }

    public void Pulled(IsoAttackManager player = null)
    {
        //animator.SetTrigger("Stop");
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        animator.enabled = true;
        animator.SetTrigger("Start");
    }
}
