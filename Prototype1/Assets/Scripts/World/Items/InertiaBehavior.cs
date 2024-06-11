using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InertiaBehavior : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The rigidbody's initial mass")] float startingMass = 1f;
    [SerializeField]
    [Tooltip("The rigidbody's final mass")] float endingMass = 1f;
    [SerializeField]
    [Tooltip("The rigidbody's initial mass")] public float massDuration = 1.5f;
    public float massTimer = 0f;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public float CalculateMass()
    {
        rb.mass = Mathf.Lerp(startingMass, endingMass, massTimer / massDuration);
        Debug.Log(rb.mass);
        Debug.Log(massTimer);
        return rb.mass;
    }
}
