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

    private bool increase = true;

    // Start is called before the first frame update
    void Start()
    {
        barrierMat = GetComponent<SkinnedMeshRenderer>().material;

        barrierMat.SetFloat("_BarrierGrow", grow);
    }

    // Update is called once per frame
    void Update()
    {
        if (grow > barrierStop)
            increase = false;
        if (grow < barrierStart)
            increase = true;

        if (increase)
            grow = Mathf.Lerp(grow, barrierStop + 1, (growSpeed / 10) * Time.deltaTime);
        else
            grow = Mathf.Lerp(grow, barrierStart - 1, (growSpeed / 10) * Time.deltaTime);

        if(barrierMat != null)
            barrierMat.SetFloat("_BarrierGrow", grow);
    }

}
