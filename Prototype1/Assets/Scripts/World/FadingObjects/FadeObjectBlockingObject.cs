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
    private bool keepShadows = true;
    [SerializeField]
    private Vector3 targetPositionOffset = Vector3.zero;
    [SerializeField]
    private float fadeSpeed = 5f;
    [SerializeField]
    private float fadeAOEX = 0.01f;
    [SerializeField]
    private float fadeAOEY = 0.01f;
    [SerializeField]
    private float fadeAOEZ = 0.01f;

    [Header("Read Only Data")]
    [SerializeField]
    private List<FadingObject> objectsBlockingView = new List<FadingObject>();
    private Dictionary<FadingObject, Coroutine> runningCoroutines = new Dictionary<FadingObject, Coroutine>();

    private RaycastHit[] hits;
    private List<FadingObject> hitFadingObjects;

    private void Start()
    {
        cam = Camera.main;
        target = transform;
        hits = new RaycastHit[100];
        hitFadingObjects = new List<FadingObject>();
        StartCoroutine(CheckForObjects());
    }

    private IEnumerator CheckForObjects()
    {
        while (true)
        {
            int targets = Physics.BoxCastNonAlloc(
                cam.transform.position,
                new Vector3 (fadeAOEX, fadeAOEY, fadeAOEZ),
                (target.transform.position + targetPositionOffset - cam.transform.position).normalized,
                hits,
                Quaternion.identity,
                Vector3.Distance(cam.transform.position, target.transform.position + targetPositionOffset),
                layermask);

            ExtDebug.DrawBoxCastBox(cam.transform.position,
                new Vector3(fadeAOEX, fadeAOEY, fadeAOEZ),
                Quaternion.identity,
                (target.transform.position + targetPositionOffset - cam.transform.position).normalized,
                Vector3.Distance(cam.transform.position, target.transform.position + targetPositionOffset),
                Color.red);


            //Debug.Log("Target's hit: " + targets);
            if (targets > 0)
            {
                Dictionary<FadingObject, float> tempDic = new Dictionary<FadingObject, float>();
                for (int i = 0; i<targets; i++)
                {
  
                    RaycastHit hit = hits[i];
                    FadingObject fadingObject = hit.transform.GetComponent<FadingObject>();
                    if (fadingObject != null && fadingObject.enabled)
                    {
                        tempDic.Add(fadingObject, hit.distance);
                        hitFadingObjects.Add(fadingObject);
                        if (!objectsBlockingView.Contains(fadingObject))
                        {
                            //Debug.Log("Starting Fading Coroutine on: " + fadingObject.name);
                            if (runningCoroutines.ContainsKey(fadingObject))
                            {
                                if (runningCoroutines[fadingObject] != null)
                                {
                                    StopCoroutine(runningCoroutines[fadingObject]);

                                }
                                runningCoroutines.Remove(fadingObject);
                            }
                            runningCoroutines.Add(fadingObject, StartCoroutine(FadeObjectOut(fadingObject)));
                            objectsBlockingView.Add(fadingObject);
                        }

                    }

                   
                    

                }
                foreach (FadingObject obj in objectsBlockingView)
                    obj.lastHit = false;
                float greatestValue = -1;
                foreach (float value in tempDic.Values)
                    if (value > greatestValue) greatestValue = value;
                if(greatestValue!=-1)
                    foreach (var (key, value) in tempDic)
                    {
                        if (greatestValue == value) key.lastHit = true;
                    }
            }

            FadeObjectsNoLongerBeingHit();

            ClearHits();

            yield return null;
        }
      
    }

    private void FadeObjectsNoLongerBeingHit()
    {
        List<FadingObject> objectsToRemove = new List<FadingObject>();
        foreach(FadingObject fadingObject in objectsBlockingView)
        {
            if(!hitFadingObjects.Contains(fadingObject))
            {
                if (runningCoroutines.ContainsKey(fadingObject))
                {
                    if(runningCoroutines[fadingObject] != null)
                    {
                        StopCoroutine(runningCoroutines[fadingObject]);
                    }
                    runningCoroutines.Remove(fadingObject);
                }
                runningCoroutines.Add(fadingObject, StartCoroutine(FadeObjectIn(fadingObject)));
                objectsToRemove.Add(fadingObject);
            }
        }
        foreach(FadingObject fadingObject in objectsToRemove)
        {
            objectsBlockingView.Remove(fadingObject);
        }
    }


    private void ClearHits()
    {
        System.Array.Clear(hits, 0, hits.Length);
        hitFadingObjects.Clear();
    }

    private void FadeLayer(GameObject fadingObject, bool isFadingOut)
    {
        if (isFadingOut)
        {   
            if (fadingObject.layer == LayerMask.NameToLayer("Ground"))
            {
                fadingObject.layer = LayerMask.NameToLayer("Ground_Transparent");
            }
            else if (fadingObject.layer == LayerMask.NameToLayer("Default"))
            {
                fadingObject.layer = LayerMask.NameToLayer("Default_Transparent");
            }
            else if (fadingObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                fadingObject.layer = LayerMask.NameToLayer("Enemy_Transparent");
            }
            else if (fadingObject.layer == LayerMask.NameToLayer("Interactables"))
            {
                fadingObject.layer = LayerMask.NameToLayer("Interactables_Transparent");
            }
        }
        else
        {
            if (fadingObject.layer == LayerMask.NameToLayer("Ground_Transparent"))
            {
                fadingObject.layer = LayerMask.NameToLayer("Ground");
            }
            else if (fadingObject.layer == LayerMask.NameToLayer("Default_Transparent"))
            {
                fadingObject.layer = LayerMask.NameToLayer("Default");
            }
            else if (fadingObject.layer == LayerMask.NameToLayer("Enemy_Transparent"))
            {
                fadingObject.layer = LayerMask.NameToLayer("Enemy");
            }
            else if (fadingObject.layer == LayerMask.NameToLayer("Interactables_Transparent"))
            {
                fadingObject.layer = LayerMask.NameToLayer("Interactables");
            }
        }
    }

    private IEnumerator FadeObjectOut(FadingObject fadingObject)
    {
        //Debug.Log("Begin fading out: " + fadingObject.transform.name);
        foreach (Material material in fadingObject.materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.SetInt("_Surface", 1);

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            material.SetShaderPassEnabled("DepthOnly", false);
            material.SetShaderPassEnabled("SHADOWCASTER", keepShadows);

            material.SetOverrideTag("RenderType", "Transparent");

            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
           
        }
        foreach(FadingObject temp in fadingObject.childrenFadingObjects)
        {
            temp.enabled = false;
            if (runningCoroutines.ContainsKey(temp))
            {
                if (runningCoroutines[temp] != null)
                {
                    StopCoroutine(runningCoroutines[temp]);
                }
                runningCoroutines.Remove(temp);
            }
            if(objectsBlockingView.Contains(temp))
            {
                objectsBlockingView.Remove(temp);
            }
            
        }
        FadeLayer(fadingObject.gameObject, true);
        foreach (Transform child in fadingObject.transform)
            FadeLayer(child.gameObject, true);
        foreach (GameObject moveable in fadingObject.moveables)
            FadeLayer(moveable, true);
        float time = 0;

        if (fadingObject.lastHit)
        {
            while (fadingObject.materials[0].color.a > fadeAlpha)
            {
                foreach (Material material in fadingObject.materials)
                {
                    if(material.HasProperty("_Color"))
                    {
                        material.color = new Color(
                            material.color.r,
                            material.color.g,
                            material.color.b,
                            Mathf.Lerp(fadingObject.initialAlpha, fadeAlpha, time * fadeSpeed)
                            );
                    }
                }
                time += Time.deltaTime;
                yield return null;
            }    
        }
        else
        {
            while (fadingObject.materials[0].color.a > 0)
            {
                foreach (Material material in fadingObject.materials)
                {
                    if (material.HasProperty("_Color"))
                    {
                        material.color = new Color(
                            material.color.r,
                            material.color.g,
                            material.color.b,
                            Mathf.Lerp(fadingObject.initialAlpha, 0, time * fadeSpeed)
                        );
                    }
                }
                time += Time.deltaTime;
                yield return null;
            }
        }
        if(runningCoroutines.ContainsKey(fadingObject))
        {
            StopCoroutine(runningCoroutines[fadingObject]);
            runningCoroutines.Remove(fadingObject);
            
        }

            
    }

    private IEnumerator FadeObjectIn(FadingObject fadingObject)
    {
        //Debug.Log("Begin fading in: " + fadingObject.transform.name);
        float time = 0;
        float startingAlpha = fadingObject.materials[0].color.a;
        while (fadingObject.materials[0].color.a < fadingObject.initialAlpha)
        {
            foreach (Material material in fadingObject.materials)
            {
                if (material.HasProperty("_Color"))
                {
                    material.color = new Color(
                        material.color.r,
                        material.color.g,
                        material.color.b,
                        Mathf.Lerp(startingAlpha, 1, time * fadeSpeed)
                    );
                }
            }
            time += Time.deltaTime;
            yield return null;
        }

        foreach (Material material in fadingObject.materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.SetInt("_Surface", 0);

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

            material.SetShaderPassEnabled("DepthOnly", true);
            material.SetShaderPassEnabled("SHADOWCASTER", true);

            material.SetOverrideTag("RenderType", "Opaque");

            material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
          
        }

        foreach (FadingObject temp in fadingObject.childrenFadingObjects)
        {
            temp.enabled = false;
            if (runningCoroutines.ContainsKey(temp))
            {
                if (runningCoroutines[temp] != null)
                {
                    StopCoroutine(runningCoroutines[temp]);
                }
                runningCoroutines.Remove(temp);
            }
            
        }
        FadeLayer(fadingObject.gameObject, false);
        foreach (Transform child in fadingObject.transform)
            FadeLayer(child.gameObject, false);
        foreach (GameObject moveable in fadingObject.moveables)
            FadeLayer(moveable, false);



        if (runningCoroutines.ContainsKey(fadingObject))
        {
            StopCoroutine(runningCoroutines[fadingObject]);
            runningCoroutines.Remove(fadingObject);
            
        }


    }
}


