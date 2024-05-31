using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTypeContainer : MonoBehaviour
{
    [SerializeField] GroundTypes groundType;
    public GroundTypes GetGroundType()
    {
        return groundType;
    }
}
