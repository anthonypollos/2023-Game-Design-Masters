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
    public void TakeDamage(int dmg)
    {
        //If there is a blood particle, create it.
        if (bloodParticle != null)
        {
            //create the particle
            GameObject vfxobj = Instantiate(bloodParticle, gameObject.transform.position, Quaternion.identity);
            //destroy the particle
            Destroy(vfxobj, 4);
        }
        jukebox.PlaySound(0);
        if (!DeveloperConsole.instance.godMode)
        {
            health -= dmg;
        }
        hpBar.value = (float)health / (float)maxHealth;
        if (health <= 0) Die();
        else anim.SetTrigger("Damage");

    }

    private void Die()
    {
        if (!pc.isDead)
        {
            anim.SetTrigger("death");
            jukebox.PlaySound(1);
            pc.isDead = true;
            GameController.instance.Lose();
        }
    }
}
