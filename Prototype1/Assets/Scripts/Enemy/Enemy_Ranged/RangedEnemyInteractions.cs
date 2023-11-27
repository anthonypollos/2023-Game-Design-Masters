using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RangedEnemyInteractions : EnemyInteractionBehaviorTemplate
{
    [SerializeField]
    float pullTimeToKill = 2f;
    [SerializeField] Slider pullSlider;
    float currentTime;
    void Start()
    {
        currentTime = 0f;
        pullSlider.gameObject.SetActive(false);
        lassoed = false;
        stunned = false;
        launched = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void Kicked()
    {
        UnStunned();

    }

    public override void Lassoed()
    {
        lassoed = true;
        base.Lassoed();
        Stunned();
        brain.an.SetBool("Lassoed", true);
    }

    public override void Pulled()
    {
        base.Pulled();
        pullSlider.gameObject.SetActive(true);
        currentTime += Time.fixedDeltaTime;
        pullSlider.value = currentTime / pullTimeToKill;
        if (currentTime >= pullTimeToKill) { PulledOut(); }
    }

    private void PulledOut()
    {
        brain.health.TakeDamage(9999999);
    }

    public override void Break()
    {
        base.Break();
        currentTime = 0;
        pullSlider.gameObject.SetActive(false);
        lassoed = false;
        brain.an.SetBool("Lassoed", false);
        UnStunned();
    }

    public override void Stagger()
    {
        base.Stagger();
    }

    protected override void Stunned()
    {
        base.Stunned();
        stunned = true;
        brain.an.SetBool("Stunned", true);
        brain.an.SetBool("Attacking", false);
    }

    protected override void UnStunned()
    {
        if (!lassoed && !launched && !brain.moveable.isLaunched)
        {
            brain.an.SetBool("Stunned", false);
            base.UnStunned();
        }
    }

    protected override IEnumerator Staggered()
    {
        StopCoroutine(base.Staggered());
        StartCoroutine(base.Staggered());
        yield break;

    }

    protected override void Stun(float time)
    {
        base.Stun(time);
    }

    protected override IEnumerator StunTimer(float seconds)
    {
        StopCoroutine(base.StunTimer(seconds));
        StartCoroutine(base.StunTimer(seconds));
        yield break;
    }
}
