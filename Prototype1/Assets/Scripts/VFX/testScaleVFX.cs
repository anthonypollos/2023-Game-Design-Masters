using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScaleVFX : MonoBehaviour
{
    public GameObject vfx;

    public float scaleMod;

    private SpawnVFX spawnVFX;

    // Start is called before the first frame update
    void Start()
    {
        spawnVFX = GetComponent<SpawnVFX>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            spawnVFX.SpawnScaledVFX(vfx, transform, scaleMod);
        }
    }
}
