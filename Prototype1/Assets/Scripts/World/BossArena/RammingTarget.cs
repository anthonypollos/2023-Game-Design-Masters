using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RammingTarget : MonoBehaviour, ITrap
{
    [SerializeField][Tooltip("Match the # of the associated trigger in the animator connected to the BossFightManager")] int targetValue;
    [SerializeField] BossFightManager bfm;

    public void ActivateTrap(GameObject target)
    {
        BossEnemyBrain check = target.GetComponent<BossEnemyBrain>();
        if(check!=null)
        {
            bfm.TargetHit(targetValue);
            check.Calm();
        }
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
