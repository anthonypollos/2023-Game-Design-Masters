using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class TownSquareCenter : MonoBehaviour
{
    Animator animator;
    [SerializeField] string animatorParamName = "CystsLeft";
    [SerializeField] int cystsAlive = 3;
    [SerializeField] List<VoiceClip> cystDestroyedBarks;
    StudioEventEmitter emitter;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        emitter = GetComponent<StudioEventEmitter>();
    }

    public void CystDestroyed()
    {
        int param = 3 - cystsAlive;
        if(param < cystDestroyedBarks.Count)
        {
            emitter.Stop();
            emitter.ChangeEvent(cystDestroyedBarks[param].eventReference);
            emitter.Play();
            SubtitleManager.instance.StartDialog(cystDestroyedBarks[param].subtitle, emitter);
        }
        cystsAlive--;
        animator.SetInteger(animatorParamName, cystsAlive);
    }
}
