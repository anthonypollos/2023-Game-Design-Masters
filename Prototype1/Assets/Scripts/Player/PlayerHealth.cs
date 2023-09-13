using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] int health = 100;
    int maxHealth;
    PlayerController pc;
    Slider hpBar;
    GameObject deathscreen;

    private void Start()
    {
        hpBar = GetComponentInChildren<Slider>();
        deathscreen = GameObject.FindGameObjectWithTag("DeathScreen");
        deathscreen.SetActive(false);
        maxHealth = health;
    }

    private void Update()
    {
        if (transform.position.y < -20f)
            Die();
    }
    public void TakeDamage(int dmg)
    {
        health -= dmg;
        hpBar.value = (float)health / (float)maxHealth;
        if (health <= 0) Die();
    }

    private void Die()
    {
        deathscreen.SetActive(true);
        pc.dead = true;
    }
}
