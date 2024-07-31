using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnabler : MonoBehaviour
{

    [Tooltip("The objects to toggle.")] [SerializeField] private GameObject[] objects;
    [Tooltip("The time to enable each object.\nMust be the same length as objects")] [SerializeField] private float[] spawntimes;

    private IEnumerator DelayedSpawn(GameObject Enablee, float Time)
    {
        yield return new WaitForSeconds(Time);
        Enablee.SetActive(!Enablee.activeSelf);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            for (int i = 0; i < objects.Length; i++)
            {
                StartCoroutine(DelayedSpawn(objects[i], spawntimes[i]));
            }
        }
    }

}
