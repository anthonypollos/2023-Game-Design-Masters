using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackBehavior : MonoBehaviour
{
    List<GameObject> hasHit;
    [SerializeField]
    int damage;

    int LAYER1 = 7;
    int LAYER2 = 26;


    private void OnEnable()
    {
        if (hasHit == null)
        {
            hasHit = new List<GameObject>();
        }
        hasHit.Clear();
    }

    private void OnDisable()
    {
        gameObject.layer = LAYER2;
    }

    public void IgnoreAllInteractables()
    {
        //Debug.Log("Changing layer to ignore");
        gameObject.layer = LAYER2;
    }

    public void RecognizeAllInteractables()
    {
        //Debug.Log("Resetting layer");
        gameObject.layer = LAYER1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasHit.Contains(other.gameObject) && other.gameObject.CompareTag("Player")) ;
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
