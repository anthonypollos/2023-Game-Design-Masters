using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable
{

    [SerializeField] int health = 20;
    int maxHealth;
    Slider slider;
    EnemyBehavior eb;
    EnemyContainer ec;
    private void Start()
    {
        ec = FindObjectOfType<EnemyContainer>();
        maxHealth = health;
        slider = GetComponentInChildren<Slider>();
        slider.value = health / maxHealth;
        eb = GetComponent<EnemyBehavior>();
    }
    public void TakeDamage(int dmg)
    {
        //Debug.Log("dealt damage");
        health -= dmg;
        eb.Stagger();
        if (health <= 0) Die();
        //Debug.Log((float)health / maxHealth);
        //Debug.Log(health);
        slider.value = (float)health/ maxHealth;
    }

    private void Die()
    {
        ec.RemoveEnemy(gameObject);
        ec.RemoveAggro(gameObject);
        Destroy(gameObject);
    }
}
