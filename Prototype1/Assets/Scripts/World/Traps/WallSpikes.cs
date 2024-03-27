using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpikes : MonoBehaviour, ITrap
{
    [SerializeField] int dmg = 20;
    [SerializeField] float bleedTime = 3f;
    [SerializeField] [Tooltip("<=0 means infinite")] int uses = 0;

    [SerializeField] private JukeBox jukebox;

    private void Awake()
    {
        jukebox.SetTransform(transform);
        if (uses <= 0)
            uses = int.MaxValue;
    }

    public void ActivateTrap(GameObject target)
    {
        int mod = 1;
        IDamageable temp = target.GetComponent<IDamageable>();
        jukebox.PlaySound(0);
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
        jukebox.PlaySound(0);
        uses--;
        if(uses==0)
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
