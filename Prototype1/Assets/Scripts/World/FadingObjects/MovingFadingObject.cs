using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingFadingObject : MonoBehaviour
{
    [Header("ReadOnly")]
    [SerializeField]
    private FadingObject parentFade;
    private LayerMask layerMask;
    private Renderer render;
    // Start is called before the first frame update
    void Start()
    {
        parentFade = null;
        string[] temp = { "Ground", "Ground_Transparent" };
        layerMask = LayerMask.GetMask(temp);
        render = GetComponent<Renderer>();
        CheckGround();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();   
    }

    void CheckGround()
    {
        RaycastHit hit;
        bool didHit = Physics.Raycast(transform.position, -Vector3.up, out hit, 10, layerMask);
        Debug.DrawRay(transform.position, -Vector3.up * 10, Color.green);
        if (didHit)
        {
            //Debug.Log("hit: " + hit.transform.name);
            FadingObject fadingObject = hit.transform.GetComponent<FadingObject>();
            if (fadingObject != null && fadingObject != parentFade)
                ChangeParent(fadingObject);
        }
        
    }

    void ChangeParent(FadingObject newParent)
    {
        foreach (Material material in render.materials)
        {
            if (parentFade != null)
            {
                parentFade.materials.Remove(material);
                parentFade.moveables.Remove(gameObject);
            }
            newParent.materials.Add(material);
            newParent.moveables.Add(gameObject);
            material.color = new Color(material.color.r, material.color.g, material.color.b,
                newParent.materials[0].color.a);
            parentFade = newParent;
            FadeLayer(newParent.gameObject.layer == LayerMask.NameToLayer("Ground_Transparent"));
        }
    }

    private void FadeLayer(bool isFadingOut)
    {
        Debug.Log("Fade Layer called: " + isFadingOut);
        if (isFadingOut)
        {
            if (gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                gameObject.layer = LayerMask.NameToLayer("Ground_Transparent");
            }
            else if (gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                gameObject.layer = LayerMask.NameToLayer("Default_Transparent");
            }
            else if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                gameObject.layer = LayerMask.NameToLayer("Enemy_Transparent");
            }
            else if (gameObject.layer == LayerMask.NameToLayer("Interactables"))
            {
                gameObject.layer = LayerMask.NameToLayer("Interactables_Transparent");
            }
        }
        else
        {
            if (gameObject.layer == LayerMask.NameToLayer("Ground_Transparent"))
            {
                gameObject.layer = LayerMask.NameToLayer("Ground");
            }
            else if (gameObject.layer == LayerMask.NameToLayer("Default_Transparent"))
            {
                gameObject.layer = LayerMask.NameToLayer("Default");
            }
            else if (gameObject.layer == LayerMask.NameToLayer("Enemy_Transparent"))
            {
                gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
            else if (gameObject.layer == LayerMask.NameToLayer("Interactables_Transparent"))
            {
                gameObject.layer = LayerMask.NameToLayer("Interactables");
            }
        }
    }
}
