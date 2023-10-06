using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chandelier : MonoBehaviour, IToggleable
{
    [SerializeField] private bool falling = false;
    public Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Toggle()
    {
        rb.useGravity = true;
        falling = !falling;
    }

    public bool GetToggle()
    {
        return falling;
    }

   
}
