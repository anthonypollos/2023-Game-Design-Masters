using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDInteractToggle : MonoBehaviour
{
    [Tooltip("UI Toggle Trigger Radius; UI Enabled when player is this far away")]
    [SerializeField] private float radius;

    [Tooltip("Which HUD interact sprite to use")]
    [SerializeField] private Image interactImage;

    private Animator anim;
    private SphereCollider col;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<SphereCollider>();

        col.radius = radius;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && anim != null)
        {
            print("player enter");

            anim.SetBool("Visible", true);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            print("player exit");

            anim.SetBool("Visible", false);
        }
    }
}
