using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeoBossFightController : MonoBehaviour
{
    int targetsHit = 0;

    [SerializeField] Animator firstTarget;
    [SerializeField] Animator secondTarget;
    [SerializeField] Animator thirdTarget;

    public void TargetHit()
    {
        targetsHit++;
    }

    public void Enrage()
    {
        switch (targetsHit)
        {
            case 0:
                firstTarget.SetTrigger("On");
                return;
            case 1:
                secondTarget.SetTrigger("On");
                return;
            case 2:
                thirdTarget.SetTrigger("On");
                return;
            default:
                print("How the fuck did you do this");
                return;
        }
    }
}
