using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    [SerializeField] barrierShader[] barriersList;

    // Start is called before the first frame update
    void Awake()
    {
        barriersList = GetComponentsInChildren<barrierShader>(true);

        ToggleBarrier(true);
    }

    public void ToggleBarrier(bool toggle)
    {
        foreach (barrierShader shader in barriersList)
        {
            shader.ToggleBarrier(toggle);
        }

        if(!toggle)
        {
            Invoke("DisableCollider", 0.4f);
            Invoke("DisableBarrier", 1f);
        }
    }

    private void DisableCollider()
    {
        GameObject collider = GetComponentInChildren<BoxCollider>().gameObject;
        collider.SetActive(false);
    }

    private void DisableBarrier()
    {
        gameObject.SetActive(false);
    }
}
