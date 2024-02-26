using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] JukeBox jukebox;
    [SerializeField] int health = 100;
    [SerializeField] GameObject bloodParticle;
    int maxHealth;
    IsoPlayerController pc;
    Slider hpBar;

    [Header("Animator Variables")]
    [SerializeField] Animator anim; //assigned in inspector for now; can change

    private void Awake()
   {
     jukebox.SetTransform(transform);
   }
    private void Start()
    {
        hpBar = GetComponentInChildren<Slider>();
        pc = GetComponent<IsoPlayerController>();
        maxHealth = health;
    }

    private void Update()
    {
        if (transform.position.y < -20f)
            Die();
    }
    public void TakeDamage(int dmg, DamageTypes damageType = DamageTypes.BLUGEONING)
    {
        //If there is a blood particle, create it.
        if (bloodParticle != null && dmg > 0)
        {
            //create the particle
            GameObject vfxobj = Instantiate(bloodParticle, gameObject.transform.position, Quaternion.identity);
            //destroy the particle
            Destroy(vfxobj, 4);
        }
        if (dmg > 0) jukebox.PlaySound(0);
        if (!DeveloperConsole.instance.godMode)
        {
            health -= dmg;
        }
        hpBar.value = (float)health / (float)maxHealth;
        if (health <= 0) Die();
        else if (dmg > 0) anim.SetTrigger("Damage");

        //Prevent overheal
        if (health > maxHealth) health = maxHealth;
    }

    public bool WillBreak(int dmg)
    {
        return (dmg >= health);
    }


    private void Die()
    {
        if (!pc.isDead)
        {
            anim.SetTrigger("Death");
            jukebox.PlaySound(1);
            pc.isDead = true;
            pc.isStunned = true;
            pc.attackState = Helpers.NOTATTACKING;
            //GameController.instance.Lose();
            StartCoroutine(Death());
        }
    }

    private bool IsAnimationFinished()
    {
        return anim.GetCurrentAnimatorClipInfo(4).Length >= 1;
    }

    public IEnumerator Death()
    {
        yield return new WaitUntil(IsAnimationFinished);
        yield return new WaitForSeconds(2);
        GameController.instance.Lose();
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
