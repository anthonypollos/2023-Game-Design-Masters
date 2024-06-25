using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownSquareCenter : MonoBehaviour
{
    Animator animator;
    [SerializeField] string animatorParamName = "CystsLeft";
    [SerializeField] int cystsAlive = 3;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void CystDestroyed()
    {
        cystsAlive--;
        animator.SetInteger(animatorParamName, cystsAlive);
    }
}
