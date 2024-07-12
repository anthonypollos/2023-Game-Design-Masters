using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerLightDistanceDisabler : MonoBehaviour
{

    [SerializeField] private float turnOnRange = 40;
    [SerializeField] private float turnOffRange = 60;
    private GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        InvokeRepeating(nameof(SlowUpdate), 0.5f, 1f);
    }

    void SlowUpdate()
    {

    }
}
