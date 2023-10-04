using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BasicMath
{
    public static Vector2 ProjectileCalc(Vector3 origin, Vector3 target, float time)
    {
        float x = 0;
        float y = 0;

        float ydif = origin.y - target.y;

        y = (-ydif + 0.5f * (-Physics.gravity.y * time*time))/time;
        origin.y = 0; target.y = 0;
        x = Vector3.Distance(origin, target) / time;
        Debug.Log("<" + x + "," + y + ">");
        return new Vector2(x, y);   
    }
}
