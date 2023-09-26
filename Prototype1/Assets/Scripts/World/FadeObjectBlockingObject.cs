using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObjectBlockingObject : MonoBehaviour
{

    [SerializeField]
    private LayerMask layermask;
    private Transform target;
    private Camera cam;
    [SerializeField]
    [Range(0f, 1f)]
    private float fadeAlpha = 0.4f;
    [SerializeField]
    private bool RetainShadows = true;
    [SerializeField]
    private Vector3 targetPositionOffset = Vector3.zero;
    [SerializeField]
    private float fadeSpeed = 5f;

    [Header("Read Only Data")]
    [SerializeField]
    private List<FadingObject> objectsBlockingView = new List<FadingObject>();
    private Dictionary<FadingObject, Coroutine> runningCoroutines = new Dictionary<FadingObject, Coroutine>();

    private RaycastHit[] hits;

    private void Start()
    {
        cam = Camera.main;
        target = transform;
        StartCoroutine(CheckForObjects());
    }

    private IEnumerator CheckForObjects()
    {
        while (true)
        {
            int targets = Physics.RaycastNonAlloc(
                cam.transform.position,
                (target.transform.position + targetPositionOffset - cam.transform.position).normalized,
                hits,
                Vector3.Distance(cam.transform.position, target.transform.position + targetPositionOffset),
                layermask);

            if (targets > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    FadingObject fadingObject = hit.transform.GetComponent<FadingObject>();

                    if(fadingObject!=null && !objectsBlockingView.Contains(fadingObject))
                    {
                        if(runningCoroutines.ContainsKey(fadingObject))
                        {
                            if (runningCoroutines[fadingObject] != null)
                            {
                                StopCoroutine(runningCoroutines[fadingObject]);

                            }

                        }
                        runningCoroutines.Remove(fadingObject);
                    }

                    runningCoroutines.Add(fadingObject, StartCoroutine(FadeObjectOut(fadingObject)));
                    objectsBlockingView.Add(fadingObject);
                }
            }

            //FadeObjectsNoLongerBeingHit();

            ClearHits();

            yield return null;
        }
      
    }

    private void ClearHits()
    {
        System.Array.Clear(hits, 0, hits.Length);
    }

    private IEnumerator FadeObjectOut(FadingObject fadingObject)
    {
        return null;
    }

}
