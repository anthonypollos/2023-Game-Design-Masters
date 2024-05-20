using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeoRammingTarget : MonoBehaviour, ITrap
{
    [SerializeField] NeoBossFightController bfm;
    [SerializeField] Animator an;
    public void ActivateTrap(GameObject target)
    {
        BossEnemyBrain check = target.GetComponent<BossEnemyBrain>();
        if (check != null)
        {
            an.SetTrigger("Hit");
            bfm.TargetHit();
            check.Calm();
        }
    }
}
