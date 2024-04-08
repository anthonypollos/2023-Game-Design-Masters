using UnityEngine;
using FMODUnity;

public class AnimSound : MonoBehaviour
{
    //[SerializeField] private JukeBox jukebox;
    [SerializeField] private EventReference enemyAttack;

    /// <summary>
    /// Plays sound clip at given index
    /// </summary>
    /// <param name="index">Index of clip to play</param>
    public void PlaySound(int index)
    {
        //jukebox.PlaySound(index);
        AudioManager.instance.PlayOneShot(enemyAttack, this.transform.position);
    }


    // figure out how to make vvvv work, can only pass one int through anim functions

    /// <summary>
    /// Plays sound clip randomly picked from given range
    /// </summary>
    /// <param name="min">Minimum clip index</param>
    /// <param name="max">Maximum clip index</param>
    public void PlaySoundRandom(int min, int max)
    {
        //jukebox.PlaySound(Random.Range(min, max + 1));
        AudioManager.instance.PlayOneShot(enemyAttack, this.transform.position);
    }
}
