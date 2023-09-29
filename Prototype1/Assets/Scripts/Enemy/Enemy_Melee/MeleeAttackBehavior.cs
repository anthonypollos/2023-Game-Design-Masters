using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackBehavior : MonoBehaviour
{
    List<GameObject> hasHit;
    [SerializeField]
    int damage;


    private void OnEnable()
    {
        hasHit = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasHit.Contains(other.gameObject))
        {
            IDamageable target = other.GetComponent<IDamageable>();
            if (target != null)
            {
                hasHit.Add(other.gameObject);
                target.TakeDamage(damage);
            }
        }
    }
}
