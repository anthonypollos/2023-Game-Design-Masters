using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineCustomizer : MonoBehaviour
{
    public enum ObjectType { Enemy, Throwable, Hazard, Pickup, Player, NPC }

    [SerializeField] ObjectType objectType;

    private Outline outline;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateOutlines()
    {

    }
}
