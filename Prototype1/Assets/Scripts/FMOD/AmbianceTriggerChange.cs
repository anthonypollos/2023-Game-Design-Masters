using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbianceTriggerChange : MonoBehaviour
{
    [SerializeField] private AmbianceArea scene;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            AudioManager.instance.SetAmbianceArea(scene);
        }
    }
}
