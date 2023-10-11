using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetPreview : MonoBehaviour
{
    Vector3 startingSize;
    private void Awake()
    {
        startingSize = transform.localScale;
        transform.localScale = Vector3.zero;
    }
    public void Begin(float time)
    {
        StartCoroutine(Grow(time));
    }

    IEnumerator Grow(float time)
    {
        for(float i = 0; i<time; i+=Time.deltaTime)
        {
            yield return null;
            float scale = i / time;
            transform.localScale = new Vector3(
                startingSize.x * scale,
                startingSize.y * scale,
                startingSize.z * scale
                );
        }
        Destroy(gameObject);
    }
}
