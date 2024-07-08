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

    [SerializeField] [Tooltip("The animator that this trap uses.")] Animator modelVisualizer;
    [SerializeField] [Tooltip("How long is the death animation?")] float timeToDeath;
    [SerializeField] [Tooltip("The object that spawns on death.\nPreferably a particle system.")] GameObject DeathObject;

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

        
        if (uses<1)
        {
            if (modelVisualizer != null) StartCoroutine(DelayedBreak()); else Break();
        }
        else if (modelVisualizer != null) modelVisualizer.SetTrigger("Hit");
    }

    //Break is called if the trap does NOT have an animator
    private void Break()
    {
        if (DeathObject != null) Instantiate(DeathObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    //DelayedBreak is called if the trap DOES have an animator
    private IEnumerator DelayedBreak()
    {
        //We don't want this to be usable while the death animation is playing.
        //First, disable the collider
        GetComponent<BoxCollider>().enabled = false;
        //2nd, start the death animation
        modelVisualizer.SetTrigger("Die");
        //3rd, set the object to die after "timeToDeath," which is set to the length of our death anim
        yield return new WaitForSeconds(timeToDeath);
        //finally, make the death object and destroy self.
        if (DeathObject != null) Instantiate(DeathObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
