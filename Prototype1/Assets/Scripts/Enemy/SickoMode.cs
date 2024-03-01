using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SickoMode : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(SafetyDelay());
    }

    IEnumerator SafetyDelay()
    {
        yield return new WaitForEndOfFrame();
        EnemyBrain brain = GetComponent<EnemyBrain>();
        if (brain != null)
            brain.Aggro();
    }
}
