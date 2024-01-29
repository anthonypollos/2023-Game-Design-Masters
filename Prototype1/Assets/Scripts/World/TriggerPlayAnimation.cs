using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Purpose: A trigger volume that starts an animation
 * Author: Sean Lee 1/29/24
 */

public class TriggerPlayAnimation : MonoBehaviour
{

    [Tooltip("The object to animate.")]
    [SerializeField] GameObject AnimatedObject;
    [Tooltip("The name of the variable in the animator to start the animation\nNote: This NEEDS to be the same as the name in the animator because Unity sucks.")]
    [SerializeField] string ObjAnimation;
    //[Tooltip("Does the animation play once and stay on the last frame?")]
    //[SerializeField] bool StayOut = true;

    // Start is called before the first frame update
    void Start()
    {
    }

    //Fires when the player enters this trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") PlayAnimation();
    }

    // Update is called once per frame
    void PlayAnimation()
    {
        AnimatedObject.GetComponent<Animator>().SetBool(ObjAnimation, true);
    }
}
