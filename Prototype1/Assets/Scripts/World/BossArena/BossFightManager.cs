using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightManager : MonoBehaviour
{
    int targetsHit = 0;
    [SerializeField] Animator animator;
    public void TargetHit(int value)
    {
        targetsHit++;
        animator.Play("SpawnAnimation" + value);
    }
}
