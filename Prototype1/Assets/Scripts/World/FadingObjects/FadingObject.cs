using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FadingObject : MonoBehaviour, IEquatable<FadingObject>
{
    [Tooltip("Leave blank for all renderers to be used")]
    public List<Renderer> renderers = new List<Renderer>();
    [HideInInspector]
    public List<Material> materials = new List<Material>();
    [HideInInspector]
    public float initialAlpha;
    [HideInInspector]
    public bool lastHit;
    [HideInInspector]
    public List<GameObject> moveables = new List<GameObject>();

    private void Awake()
    {
        lastHit = false;

        if(renderers.Count == 0)
        {
            renderers.AddRange(gameObject.GetComponentsInChildren<Renderer>());
        }

        foreach(Renderer renderer in renderers)
        {
            materials.AddRange(renderer.materials);
        }

        initialAlpha = materials[0].color.a;

    }

    public bool Equals(FadingObject other)
    {
        return string.Equals(name, other.name);
    }

    public override int GetHashCode()
    {
        return name.GetHashCode();
    }

}
