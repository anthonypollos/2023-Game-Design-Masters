using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectiveHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Animator anim;

    public void OnPointerEnter(PointerEventData eventData)
    {
        anim.SetBool("MouseOver", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anim.SetBool("MouseOver", false);
    }
}
