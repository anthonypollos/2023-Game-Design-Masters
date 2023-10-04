using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SporeCloud : MonoBehaviour
{
    [SerializeField]
    float tickInterval = 0.5f;
    [SerializeField]
    int damagePerTick = 5;
    [SerializeField]
    float duration = 4f;

    float hitCD;
    float time;

    PlayerHealth health;

    private void Start()
    {
        hitCD = tickInterval;
        health = null;
        time = 0f;

    }

    private void Update()
    {
        hitCD += Time.deltaTime;
        time += Time.deltaTime;
        if(time>duration) { Destroy(gameObject); }
    }

    private void OnTriggerStay(Collider other)
    {
        if(hitCD >= tickInterval && other.CompareTag("Player"))
        {
            if(health == null) health = other.GetComponent<PlayerHealth>();

            hitCD = 0;
            health.TakeDamage(damagePerTick);
        }
    }
}
