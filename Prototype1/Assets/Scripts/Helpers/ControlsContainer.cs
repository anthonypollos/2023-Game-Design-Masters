using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsContainer : MonoBehaviour, ISaveable
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

    public void SaveData(ref SavedValues savedValues)
    {
        //mainControls.Disable();
        //mainControls = new MainControls();
        //mainControls.Enable();
    }

    public void LoadData(SavedValues savedValues)
    {
        //throw new System.NotImplementedException();
    }

}
