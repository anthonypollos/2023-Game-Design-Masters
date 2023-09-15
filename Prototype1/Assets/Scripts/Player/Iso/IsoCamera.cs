using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoCamera : MonoBehaviour
{
    private Transform player;
    public float _smoothing = 5f;
    Vector3 _offset;

    List<Transparency> currentFade; 
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentFade = new List<Transparency>();
        _offset = transform.position - player.position;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        RaycastHit[] hits;
        Vector3 dir = player.position - transform.position;
        List<Transparency> newFades = new List<Transparency>();

        hits = Physics.RaycastAll(transform.position, dir.normalized, dir.magnitude);
        foreach (RaycastHit hit in hits)
        {
            //Debug.Log(hit.transform.name);
            Transparency temp = hit.transform.GetComponent<Transparency>();
            //Debug.Log(temp != null);
            if(temp != null)
            {
                temp.DoFade(true);
                newFades.Add(temp);
            }
        }
        foreach (Transparency temp in currentFade)
        {
            if(!newFades.Contains(temp))
            {
                temp.DoFade(false);
            }
        }
        currentFade = newFades;
        
    }

    void LateUpdate()
    {
        Vector3 targetCamPos = player.position + _offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, _smoothing * Time.deltaTime);
    }
}
