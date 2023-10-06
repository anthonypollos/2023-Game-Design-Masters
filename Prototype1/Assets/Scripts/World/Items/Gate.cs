using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour, IToggleable
{
    [SerializeField] private bool open = false;
    private Animator an;
   

    // Start is called before the first frame update
    void Start()
    {
        an = GetComponent<Animator>();
        an.SetBool("Open", open);
        an.speed = 999;
    }

    
    public void Toggle()
    {
        an.speed = 1;
        open = !open;
        an.SetBool("Open", open);
        
    }

    public bool GetToggle()
    {
        return open;
    }
}
