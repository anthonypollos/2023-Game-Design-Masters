using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FadingObject : MonoBehaviour, IEquatable<FadingObject>
{
    [Tooltip("Leave blank for all renderers to be used")]
    public List<Renderer> renderers = new List<Renderer>();
    public Vector3 position;
    [HideInInspector]
    public List<Material> materials = new List<Material>();
    [HideInInspector]
    public float InitialAlpha;

    private void Awake()
    {
        position = transform.position; 

        if(renderers.Count == 0)
        {
            renderers.AddRange(gameObject.GetComponentsInChildren<Renderer>());
        }

        foreach(Renderer renderer in renderers)
        {
            materials.AddRange(renderer.materials);
        }

        InitialAlpha = materials[0].color.a;

    }

    public bool Equals(FadingObject other)
    {
        return position.Equals(other.position);
    }

    public override int GetHashCode()
    {
        return position.GetHashCode();
    }

}
