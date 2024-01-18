using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private JukeBox jukebox;
    [SerializeField] int health = 100;
    [SerializeField] int staggerThreshold = 15;
    int maxHealth;
    Slider slider;
    [HideInInspector]
    public EnemyContainer ec;
    [HideInInspector]
    public EnemyBrain brain;

    private void Awake()
    {
        jukebox.SetTransform(transform); 
    }
    private void Start()
    {
        ec = FindObjectOfType<EnemyContainer>();
        ec.AddEnemy(gameObject);
        maxHealth = health;
        slider = GetComponentInChildren<Slider>();
        slider.value = health / maxHealth;
    }
    private void Update()
    {
        if (transform.position.y < -20f)
            Die();
    }
    public void TakeDamage(int dmg)
    {
        //Debug.Log("dealt damage");
        health -= dmg;
        if(dmg>staggerThreshold)
            brain.interaction.Stagger();
        if (health <= 0) Die();
        //Debug.Log((float)health / maxHealth);
        //Debug.Log(health);
        slider.value = (float)health/ maxHealth;
        jukebox.PlaySound(0);
    }

    public bool WillBreak(int dmg)
    {
        return (dmg >= health);
    }

    private void Die()
    {
        jukebox.PlaySound(1);
        ec.RemoveEnemy(gameObject);
        ec.RemoveAggro(gameObject);
        Destroy(gameObject);
    }
}
