using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{

    [field: Header("Music")]
    [field:SerializeField] public EventReference music { get; private set; }

    [field: Header("Ambiance")]
    [field: SerializeField] public EventReference ambiance { get; private set; }
    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMODEvents script in the scene");
        }
        instance = this;
    }

}
