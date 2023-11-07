using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class EnemyInteractionBehaviorTemplate : MonoBehaviour, IPullable, IKickable
{
    [HideInInspector]
    protected bool lassoed;
    [HideInInspector]
    protected bool hasCollided;
    [HideInInspector]
    public bool stunned;
    [HideInInspector]
    public EnemyBrain brain;
    [SerializeField] float breakOutTime = 5f;
    [SerializeField] Image lassoImage;
    [HideInInspector]
    public IsoAttackManager lassoOwner;
    
    

    public abstract void Kicked();
    public virtual void Lassoed()
    {
        lassoImage.fillAmount = 0;
        lassoImage.gameObject.SetActive(true);
        StartCoroutine(BreakOut());
    }
    public virtual void Pulled()
    {
        //lassoImage.gameObject.SetActive(false);
    }
    public virtual void Break()
    {
        lassoImage.gameObject.SetActive(false);
        if (lassoOwner != null) lassoOwner.ForceRelease();
    }
    public abstract void Stagger();
    protected virtual void Stunned()
    {
        brain.state = EnemyStates.NOTHING;
    }
    protected abstract void UnStunned();

    protected IEnumerator BreakOut()
    {
        float timer = 0;
        while(lassoed)
        {
            timer += Time.deltaTime;
            lassoImage.fillAmount = timer / breakOutTime;
            if (timer >= breakOutTime) Break();
            yield return null;
        }
    }

}
