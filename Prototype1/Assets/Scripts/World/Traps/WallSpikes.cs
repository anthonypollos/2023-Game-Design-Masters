using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class WallSpikes : MonoBehaviour, ITrap
{
    [SerializeField] int dmg = 20;
    [SerializeField] float bleedTime = 3f;
    [SerializeField] [Tooltip("<=0 means infinite")] int uses = 0;

    //[SerializeField] private JukeBox jukebox;
    [SerializeField] private EventReference stab;

    [SerializeField] private float red = 0f;
    [SerializeField] private float green = 1f;

    private void Awake()
    {
        //jukebox.SetTransform(transform);
        if (uses <= 0)
            uses = int.MaxValue;
    }

    public void ActivateTrap(GameObject target)
    {
        int mod = 1;
        IDamageable temp = target.GetComponent<IDamageable>();
        //jukebox.PlaySound(0);
        AudioManager.instance.PlayOneShot(stab, this.transform.position);
        if (temp!= null)
        {
            if(target.CompareTag("Player"))
                mod = 2;
            temp.TakeDamage(dmg/mod);
        }
        Bleedable bleedable = target.GetComponent<Bleedable>();
        if(bleedable!=null)
        {
            bleedable.Activate(bleedTime/mod);
        }
        //jukebox.PlaySound(0);
        AudioManager.instance.PlayOneShot(stab, this.transform.position);
        uses--;

        for (int i = 0; i < uses; i++)
        {
            red = red * (1.25f);
            green = green * (.75f);
            GetComponent<Renderer>().material.color = new Color(red, green, 0, 0);
        }
        if (uses==0)
        {
            Break();
        }
    }

    private void Break()
    {
        //Play breaking animation/sound here
        Destroy(gameObject);
    }
}
