using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class WallSpikes : MonoBehaviour, ITrap
{
    [SerializeField] int dmg = 20;
    [SerializeField] float bleedTime = 3f;
    [SerializeField] [Tooltip("<=0 means infinite")] int uses = 0;
    [SerializeField] [Tooltip("(Enemy Only) How long a stabbed enemy is stunned for")] float stunTime = 1f;
    //[SerializeField] private JukeBox jukebox;
    [SerializeField] private EventReference stab;

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


        //If an enemy hits this, stun them for an amount of time
        if (target.CompareTag("Enemy"))
        {
            target.GetComponent<EnemyBrain>().interaction.Stun(stunTime);
        }
        //jukebox.PlaySound(0);
        AudioManager.instance.PlayOneShot(stab, this.transform.position);
        uses--;

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
