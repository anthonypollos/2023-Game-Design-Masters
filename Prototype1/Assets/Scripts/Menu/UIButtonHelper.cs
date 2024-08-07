using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonHelper : MonoBehaviour
{
    private void OnEnable()
    {
        if(gameObject.GetComponent<Button>() && gameObject.GetComponent<Button>().interactable)
        {
            if(gameObject.GetComponent<Animator>())
                gameObject.GetComponent<Animator>().SetTrigger("Normal");
        }
    }
}
