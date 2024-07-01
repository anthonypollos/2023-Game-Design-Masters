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
        //Debug.Log("<" + x + "," + y + ">");
        return new Vector2(x, y);   
    }
}

public static class GameObjectSearcher
{
    public static GameObject FindChildObjectWithTag(this GameObject parent, string tag)
    {
        foreach(Transform t in parent.transform)
        {
            if(t.CompareTag(tag))
            {
                return t.gameObject;

            }
        }
        return null;
    }

    public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag)
    {
        foreach (Transform t in parent.transform)
        {
            if (t.CompareTag(tag))
            {
                T temp = t.GetComponent<T>();
                if(temp!=null)
                {
                    return temp;
                }
            }
        }
        return default(T);
    }
}

public static class Translations
{
    public static string TranslateToSprite(this string raw)
    {
        string rawUpper = raw.ToUpper();
        switch (rawUpper)
        {
            case "A":
                return "<sprite=1>";
            case "B":
                return "<sprite=2>";
            case "C":
                return "<sprite=3>";
            case "D":
                return "<sprite=4>";
            case "E":
                return "<sprite=5>";
            case "F":
                return "<sprite=6>";
            case "G":
                return "<sprite=7>";
            case "H":
                return "<sprite=8>";
            case "I":
                return "<sprite=9>";
            case "J":
                return "<sprite=10>";
            case "K":
                return "<sprite=11>";
            case "L":
                return "<sprite=12>";
            case "M":
                return "<sprite=13>";
            case "N":
                return "<sprite=14>";
            case "O":
                return "<sprite=15>";
            case "P":
                return "<sprite=16>";
            case "Q":
                return "<sprite=17>";
            case "R":
                return "<sprite=18>";
            case "S":
                return "<sprite=19>";
            case "T":
                return "<sprite=20>";
            case "U":
                return "<sprite=21>";
            case "V":
                return "<sprite=22>";
            case "W":
                return "<sprite=23>";
            case "X":
                return "<sprite=24>";
            case "Y":
                return "<sprite=25>";
            case "Z":
                return "<sprite=26>";
            case "1":
                return "<sprite=27>";
            case "2":
                return "<sprite=28>";
            case "3":
                return "<sprite=29>";
            case "4":
                return "<sprite=30>";
            case "5":
                return "<sprite=31>";
            case "6":
                return "<sprite=32>";
            case "7":
                return "<sprite=33>";
            case "8":
                return "<sprite=34>";
            case "9":
                return "<sprite=35>";
            case "0":
                return "<sprite=36>";
            case "NUM 1":
                return "<sprite=27>";
            case "NUM 2":
                return "<sprite=28>";
            case "NUM 3":
                return "<sprite=29>";
            case "NUM 4":
                return "<sprite=30>";
            case "NUM 5":
                return "<sprite=31>";
            case "NUM 6":
                return "<sprite=32>";
            case "NUM 7":
                return "<sprite=33>";
            case "NUM 8":
                return "<sprite=34>";
            case "NUM 9":
                return "<sprite=35>";
            case "NUM 0":
                return "<sprite=36>";
            case "F1":
                return "<sprite=37>";
            case "F2":
                return "<sprite=38>";
            case "F3":
                return "<sprite=39>";
            case "F4":
                return "<sprite=40>";
            case "F5":
                return "<sprite=41>";
            case "F6":
                return "<sprite=42>";
            case "F7":
                return "<sprite=43>";
            case "F8":
                return "<sprite=44>";
            case "F9":
                return "<sprite=45>";
            case "F10":
                return "<sprite=46>";
            case "F11":
                return "<sprite=47>";
            case "F12":
                return "<sprite=48>";
            case "`":
                return "<sprite=116>";
            case "-":
                return "<sprite=50>";
            case "=":
                return "<sprite=51>";
            case "]":
                return "<sprite=52>";
            case "[":
                return "<sprite=53>";
            case "\\":
                return "<sprite=54>";
            case ";":
                return "<sprite=55>";
            case "'":
                return "<sprite=56>";
            case ".":
                return "<sprite=57>";
            case ",":
                return "<sprite=58>";
            case "UP":
                return "<sprite=59>";
            case "DOWN":
                return "<sprite=60>";
            case "LEFT":
                return "<sprite=61>";
            case "RIGHT":
                return "<sprite=62>";
            case "SPACE":
                return "<sprite=64><sprite=65>";
            case "RIGHT SHIFT":
                return "<sprite=66><sprite=67>";
            case "SHIFT":
                return "<sprite=68><sprite=69>";
            case "LEFT SHIFT":
                return "<sprite=68><sprite=69>";
            case "ENTER":
                return "<sprite=70><sprite=71>";
            case "ESCAPE":
                return "<sprite=72><sprite=73>";
            case "ALT":
                return "<sprite=74><sprite=75>";
            case "LEFT ALT":
                return "<sprite=74><sprite=75>";
            case "RIGHT ALT":
                return "<sprite=74><sprite=75>";
            case "CTRL":
                return "<sprite=76><sprite=77>";
            case "RIGHT CTRL":
                return "<sprite=76><sprite=77>";
            case "LEFT CTRL":
                return "<sprite=76><sprite=77>";
            case "DELETE":
                return "<sprite=78><sprite=79>";
            case "CAPS LOCK":
                return "<sprite=80><sprite=81>";
            case "TAB":
                return "<sprite=82><sprite=83>";
            case "LEFT BUTTON":
                return "<sprite=85>";
            case "RIGHT BUTTON":
                return "<sprite=86>";
            case "MIDDLE BUTTON":
                return "<sprite=87>";
            case "+":
                return "<sprite=114>";
            case "*":
                return "<sprite=115>";
            case "/":
                return "<sprite=117>";
            case "BACKSPACE":
                return "<sprite=118><sprite=119>";
            default:
                return "[" + rawUpper + "]";
        }

    }
}
