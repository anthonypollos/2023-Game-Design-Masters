using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickBehavior : MonoBehaviour
{
    [SerializeField] int dmg = 5;
    Animator an;
    IsoAttackManager attackManager;

    [Header("Sound")]
    [SerializeField] private JukeBox jukebox;

    private void Awake()
    {
        jukebox.SetTransform(transform);   
    }

    private void Start()
    {
        an = GetComponent<Animator>();
        attackManager = GetComponentInParent<IsoAttackManager>();
    }
    private void OnEnable()
    {
        if(an != null) 
            an.SetTrigger("Kick");
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("touch");
        if(other.gameObject.GetComponentInParent<IKickable>()!=null)
        {
            //Debug.Log("hit");
            GetComponentInParent<ICanKick>().ActivateKick(other.gameObject);
            if (other.tag == "Interactable")
            {
                jukebox.PlaySound(0);
            }
            else if (other.tag == "Enemy")
            {
                jukebox.PlaySound(1);
            }
            //Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
        if(other.gameObject.GetComponentInParent<IDamageable>()!=null)
        {
            //Debug.Log("deal damage");
            other.gameObject.GetComponentInParent<IDamageable>().TakeDamage(dmg);
        }
    }

    public void KickEnd()
    {
        attackManager.KickEnd();
    }
}
