using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHideObjects : MonoBehaviour
{
    [Tooltip("The visualizers for each of the objects we want to hide.\nNOTE: Needs to be the visualizer, not the base object")]
    [SerializeField] private GameObject[] Objects;


    private void HideAll(GameObject obj)
    {
        if (obj.GetComponent<MeshRenderer>()) obj.GetComponent<MeshRenderer>().enabled = false;
        if (obj.GetComponent<Outline>()) obj.GetComponent<Outline>().enabled = false;
        if (obj.GetComponent<Outline>()) obj.GetComponent<OutlineCustomizer>().enabled = false;
    }

    private void ShowAll(GameObject obj)
    {
        if (obj.GetComponent<MeshRenderer>()) obj.GetComponent<MeshRenderer>().enabled = true;
        if (obj.GetComponent<Outline>()) obj.GetComponent<Outline>().enabled = true;
        if (obj.GetComponent<Outline>()) obj.GetComponent<OutlineCustomizer>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject obj in Objects)
            {
                HideAll(obj);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject obj in Objects)
            {
                ShowAll(obj);
            }
        }
    }

}
