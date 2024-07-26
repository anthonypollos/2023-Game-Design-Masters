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

    private bool shouldGrow = true, shouldPulse = false, shouldDie = false;

    private void Start()
    {
        barrierMat = GetComponent<SkinnedMeshRenderer>().material;
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        grow = barrierStart;
        shouldGrow = true;
        shouldPulse = false;

        barrierMat = GetComponent<SkinnedMeshRenderer>().material;
        barrierMat.SetFloat("_BarrierGrow", grow);
        barrierMat.SetFloat("_NoiseStrength", 0);
        barrierMat.SetFloat("_Pulse", 0);
    }

    // switched from update to fixedupdate because... yknow, tying VFX to framerate is a no-no
    void FixedUpdate()
    {
        if (shouldGrow)
            Grow();

        // gradually increases Noise Strength, to make it less visually jarring
        if (shouldPulse)
        {
            float noiseStrength = 0;

            noiseStrength = Mathf.Lerp(noiseStrength, 0.7f, 0.015f * Time.deltaTime);

            barrierMat.SetFloat("_NoiseStrength", noiseStrength);

            if (noiseStrength >= 0.6f)
            {
                barrierMat.SetFloat("_NoiseStrength", noiseStrength);
                shouldPulse = false;
            }
        }

        if (shouldDie)
            Die();
        
    }

    private void Grow()
    {
        grow = Mathf.Lerp(grow, barrierStop + 1, (growSpeed / 10) * Time.deltaTime);

        if (barrierMat != null)
            barrierMat.SetFloat("_BarrierGrow", grow);

        if (grow > barrierStop)
        {
            grow = barrierStop;

            barrierMat.SetFloat("_Pulse", 1);

            shouldGrow = false;
            shouldPulse = true;
        }
    }

    private void Die()
    {
        barrierMat.SetFloat("_Pulse", 0);
        shouldPulse = false;

        grow = Mathf.Lerp(grow, barrierStart - 1, (growSpeed / 10) * Time.deltaTime);

        if (barrierMat != null)
            barrierMat.SetFloat("_BarrierGrow", grow);

        if (grow < barrierStart)
        {
            grow = barrierStart;
            shouldDie = false;
            gameObject.SetActive(false);
        }
    }

    public void ToggleBarrier(bool toggle)
    {
        if (toggle)
            shouldGrow = true;
        else if (!toggle)
            shouldDie = true;
    }
}
