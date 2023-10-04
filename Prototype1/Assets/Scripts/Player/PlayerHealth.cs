using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] int health = 100;
    int maxHealth;
    IsoPlayerController pc;
    Slider hpBar;

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
        health -= dmg;
        hpBar.value = (float)health / (float)maxHealth;
        if (health <= 0) Die();
    }

    private void Die()
    {
        pc.isDead = true;
        GameController.instance.Lose();
    }
}
