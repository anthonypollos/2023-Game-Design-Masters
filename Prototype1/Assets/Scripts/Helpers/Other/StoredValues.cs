using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[SerializeField]
public enum DamageTypes
{
    BLUGEONING = 1, EXPLOSION = 2, ENERGY = 3, PIERCING = 4, FIRE = 5
}
public static class StoredValues
{
    public static string[] MovableTagsToIgnore = new string[] { "Ground", "Lasso", "FloorHazard" };
}
