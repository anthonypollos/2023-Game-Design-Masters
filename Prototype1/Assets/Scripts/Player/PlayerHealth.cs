using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] JukeBox jukebox;
    [SerializeField] int health = 100;
    int maxHealth;
    IsoPlayerController pc;
    Slider hpBar;

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
        jukebox.PlaySound(0);
        health -= dmg;
        hpBar.value = (float)health / (float)maxHealth;
        if (health <= 0) Die();
    }

    private void Die()
    {
        jukebox.PlaySound(1);
        pc.isDead = true;
        GameController.instance.Lose();
    }
}
