using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public abstract class InteractableBehaviorTemplate : MonoBehaviour
{
    [SerializeField] [Tooltip("Set your string to work after the words \"Press _ to .....\"")]protected string interactionText;


    public virtual string Activate()
    {
        return interactionText;

    }


    //Bool = if item is destroyed after it is interacted with
    public abstract bool Interact();

}
