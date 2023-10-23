using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScaleVFX : MonoBehaviour
{
    public GameObject vfx;

    public float scaleMod;

    private VFXSpawner vfxSpawner;

    // Start is called before the first frame update
    void Start()
    {
        vfxSpawner= GetComponent<VFXSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            vfxSpawner.SpawnVFX(vfx, transform);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            vfxSpawner.SpawnVFXScale(vfx, transform, 2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            vfxSpawner.SpawnVFXScaleRandom(vfx, transform, 1, 4);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            vfxSpawner.SpawnVFX(vfx, transform, 3);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            vfxSpawner.SpawnVFXScale(vfx, transform, 2, 3);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            vfxSpawner.SpawnVFXScaleRandom(vfx, transform, 1, 4, 3);
        }

    }
}
