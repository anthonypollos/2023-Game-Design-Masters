using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barrierShader : MonoBehaviour
{
    private Material barrierMat;

    [SerializeField]
    private float grow = 0;

    [SerializeField]
    private float barrierStart, barrierStop;

    [SerializeField]
    private float growSpeed;

    [SerializeField]
    private bool loop;

    private bool increase = true;

    // Start is called before the first frame update
    void Start()
    {
        barrierMat = GetComponent<SkinnedMeshRenderer>().material;

        barrierMat.SetFloat("_BarrierGrow", grow);
    }

    // switched from update to fixedupdate because... yknow, tying VFX to framerate is a no-no
    void FixedUpdate()
    {
        if (grow > barrierStop)
            increase = false;
        if (grow < barrierStart)
            increase = true;
        if (loop)
        {
            if (increase)
                grow = Mathf.Lerp(grow, barrierStop + 1, (growSpeed / 10) * Time.deltaTime);
            else
                grow = Mathf.Lerp(grow, barrierStart - 1, (growSpeed / 10) * Time.deltaTime);
        }
        else
        {
            grow = Mathf.Lerp(grow, barrierStart - 1, (growSpeed / 10) * Time.deltaTime);
        }
        if(barrierMat != null)
            barrierMat.SetFloat("_BarrierGrow", grow);
    }

}
