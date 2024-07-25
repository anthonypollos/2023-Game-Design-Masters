using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FMODUnity;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    //[SerializeField] JukeBox jukebox;
    [SerializeField][Tooltip("The player's current health value")] int health = 100;
    [SerializeField] GameObject bloodParticle;
    [SerializeField] int maxHealth = 100;
    IsoPlayerController pc;
    //Slider hpBar;
    [SerializeField] private Image healthFill;

    [Header("Animator Variables")]
    [SerializeField] Animator anim; //assigned in inspector for now; can change
    [SerializeField] private EventReference playerDamage1;
    [SerializeField] private EventReference playerDamage2;
    [SerializeField] private EventReference playerDamage4;
    [SerializeField] private EventReference playerDamage5;
    [SerializeField] private EventReference hurtsound;
    [SerializeField] private EventReference playerDeath;
    private void Awake()
   {
     //jukebox.SetTransform(transform);
   }
    private void Start()
    {
        //hpBar = GetComponentInChildren<Slider>();
        pc = GetComponent<IsoPlayerController>();
        //maxHealth = health;
        //hpBar.value = (float)health / (float)maxHealth;
        healthFill.fillAmount = (float)health / (float)maxHealth;
        DeveloperConsole.instance.SetHealth(this);
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
        if (dmg > 0) PickEffortSound(hurtsound, Random.Range(1, 5));
        if (!DeveloperConsole.instance.godMode)
        {
            health -= dmg;
        }
        //hpBar.value = (float)health / (float)maxHealth;
        healthFill.fillAmount = (float)health / (float)maxHealth;
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
            AudioManager.instance.PlayOneShot(playerDeath, this.transform.position);
            //jukebox.PlaySound(1);
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
        //yield return new WaitUntil(IsAnimationFinished);
        //yield return new WaitForSeconds(2);
        yield return null;
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

    public void PickEffortSound(EventReference hurtsound, int selection)
    {
        print(selection);
        switch (selection)
        {
            case 4:
                hurtsound = playerDamage5;
                AudioManager.instance.PlayOneShot(hurtsound, this.transform.position);
                break;
            case 3:
                hurtsound = playerDamage4;
                AudioManager.instance.PlayOneShot(hurtsound, this.transform.position);
                break;
            case 2:
                hurtsound = playerDamage2;
                AudioManager.instance.PlayOneShot(hurtsound, this.transform.position);
                break;
            default:
                hurtsound = playerDamage1;
                AudioManager.instance.PlayOneShot(hurtsound, this.transform.position);
                break;
        }
    }
}
