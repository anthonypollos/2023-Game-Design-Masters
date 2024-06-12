using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Josh Bonovich
//This is for all objects that is a field of damage on the ground that deals damage based on an objects movement


public class Caltrops : MonoBehaviour
{
    [SerializeField] int maxDamageAvalible;
    [SerializeField] float distanceToDamage;
    [SerializeField] int damagePerInstance;
    [SerializeField] bool destroyParent;
    private struct Values: IEquatable<Values>
    {
        public Values(Transform transform, IDamageable damageable)
        {
            this.transform = transform;
            this.damageable = damageable;
            this.distance = 0;
            this.previousPos = transform.position;
        }
        public Transform transform;
        public IDamageable damageable;
        public float distance;
        public Vector3 previousPos;

        public bool Equals(Values other)
        {
            return other.transform == this.transform;
        }
    }
    List<Values> values;
    // Start is called before the first frame update
    void Start()
    {
        values = new List<Values>();
    }

    private void FixedUpdate()
    {
        List<Values> itemsToRemove = new List<Values> ();
        for (int i = 0; i < values.Count; i++) 
        {
            Values v = values[i];
            if (v.transform == null)
            {
                itemsToRemove.Add(v);
                continue;
            }
            v.distance += Vector3.Distance(v.transform.position, v.previousPos);
            v.previousPos = v.transform.position;
            Debug.Log(v.distance);
            if(v.transform.gameObject.layer == LayerMask.NameToLayer("PlayerDashing"))
            {
                v.distance = values[i].distance;
            }
            while(v.distance>=distanceToDamage)
            {
                v.distance -= distanceToDamage;
                Debug.Log("Deal damage and subtract");
                Debug.Log(v.distance);
                v.damageable.TakeDamage(damagePerInstance);
                maxDamageAvalible -= damagePerInstance;
                if(maxDamageAvalible<=0)
                {
                    //Due to parenting and scaling, I've made the caltrops part of broken glass a child of an empty gameobject so that I could keep the scale as 1 1 1
                    //To factor this into the issue of destroying the object upon walking over it enough, I've added a boolean that destroys the parent instead
                    if (destroyParent && transform.parent != null)
                    {
                        Destroy(transform.parent.gameObject); break;
                    }
                    else Destroy(gameObject); break;
                }
            }
            values[i] = v;
        }
        foreach(Values v in itemsToRemove)
        {
            values.Remove(v);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable;
        if (other.transform.TryGetComponent<IDamageable>(out damageable))
        {
            Values value = new Values(other.transform, damageable);
            if (!values.Contains(value))
            {
                Debug.Log("Add values");
                values.Add(value);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IDamageable damageable;
        if (other.transform.TryGetComponent<IDamageable> (out damageable))
        {
            Values value = new Values(other.transform, damageable);
            if(values.Contains(value))
            {
                Debug.Log("Remove values");
                values.Remove(value);
            }
        }
    }
}
