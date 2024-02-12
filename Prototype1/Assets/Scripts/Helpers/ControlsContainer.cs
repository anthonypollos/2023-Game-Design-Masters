using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsContainer : MonoBehaviour
{
    public static ControlsContainer instance;
    public MainControls mainControls {get; private set; }
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            transform.parent = null;
            DontDestroyOnLoad(this.gameObject);
            mainControls = new MainControls();
            mainControls.Enable();
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        if (mainControls != null)
        {
            mainControls.Enable();
        }

    }

    private void OnDisable()
    {
        if(mainControls != null)
        {
            mainControls.Disable();
        }
    }
}
