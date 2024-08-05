using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class NeoRammingTarget : MonoBehaviour, IDamageable, ITrap
{
    [SerializeField] NeoBossFightController bfm;
    [SerializeField] Animator an;

    private BossEnemyBrain brain;
    bool isDestroyed = false;

    //We serialize the organ object so that we can set its anims and stuff
    [SerializeField] GameObject organ;
    [SerializeField] int health = 10;

    [SerializeField] GameObject damageParticle;
    [SerializeField] Vector3 damageParticleSpawnPosition;
    //Just gonna set this manually in here for now, I doubt we'll EVER need to change this past 3
    private float damageParticleLifetime = 3f;

    [SerializeField] GameObject overseer;
    private Animator overseerAnim;

    [Space(5)]

    [Header("Sound Settings")]

    [Tooltip("The sound that plays when a cyst is Damaged")]
    [SerializeField] protected EventReference cystDamagedSound;

    [Tooltip("The sound that plays when a cyst is Killed")]
    [SerializeField] protected EventReference cystKilledSound;

    [Tooltip("The Manually set offset after a Cyst is 'killed' for the killed sound to play")]
    [SerializeField] protected float timeToKillSound;

    [SerializeField] private VoiceClip overseerCystHitSound;
    [SerializeField] private VoiceClip overseerCystBreakSound;

    private int twothirds;
    private int onethird;
    private Animator organAnim;
    public void ActivateTrap(GameObject target)
    {
        //BossEnemyBrain check = target.GetComponent<BossEnemyBrain>();
        if (target == brain.gameObject)
        {
            OnDeath();
        }
    }

    private void Start()
    {
        brain = FindObjectOfType<BossEnemyBrain>(true);
        twothirds = health * (2 / 3);
        onethird = health / 3;
        organAnim = organ.GetComponent<Animator>();
        overseerAnim = overseer.GetComponent<Animator>();
    }

    public int GetHealth()
    {
        return 0;
    }

    public void TakeDamage(int dmg, DamageTypes damageType = DamageTypes.BLUGEONING)
    {
        health -= dmg;

        //If we have a damaged sound, play it
        AudioManager.instance.PlayOneShot(cystDamagedSound, transform.position);
        AudioManager.instance.PlayOneShot(overseerCystHitSound, transform.position);

        if (damageParticle != null)
        {
            Instantiate(damageParticle, damageParticleSpawnPosition, Quaternion.identity);
        }

        //No more HP, die
        if (health <= 0)
        {
            OnDeath();
        }
        //We DO have an organ with an animator
        if (organAnim != null)
        {
            //Still alive, play the hit animation
            if (health > 0)
            {
                organAnim.SetTrigger("Hit");
            }
            //two thirds health, start Idle 2
            if (health <= twothirds)
            {
                organAnim.SetInteger("health", 2);
            }
            //last third, start Idle 3
            if (health <= onethird)
            {
                organAnim.SetInteger("health", 1);
            }
        }
    }

    public bool WillBreak(int dmg)
    {
        return true;
    }

    private void OnDeath()
    {
        if (!isDestroyed)
        {
            isDestroyed = true;

            //If we have a cyst death sound, start the coroutine to play it after the set number of seconds we input
            StartCoroutine(DelayDeathSound());

            an.SetTrigger("Hit");
            if (organAnim != null) organ.GetComponent<Animator>().SetTrigger("Die");
            //If the overseer is attached
            if (overseerAnim)
            {
                //Overseer's "health" is reduced by one to switch to a different idle
                //We do this just before setting the hit trigger just to make sure that when the hit animation is done, it transitions to the new idle
                //This is easier than manually setting up interlaced transitions between all 4 idles.
                overseerAnim.SetInteger("health", overseerAnim.GetInteger("health") - 1);
                //Overseer plays hit animation
                overseerAnim.SetTrigger("Hit");
            }
            bfm.TargetHit();
            brain.Calm();
        }
    }

    private IEnumerator DelayDeathSound()
    {
        yield return new WaitForSeconds(timeToKillSound);
        AudioManager.instance.PlayOneShot(cystKilledSound, transform.position);
        AudioManager.instance.PlayOneShot(overseerCystBreakSound, transform.position);
    }
}
