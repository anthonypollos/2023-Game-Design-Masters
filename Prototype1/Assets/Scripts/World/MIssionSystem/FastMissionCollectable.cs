using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose: A mission collectable that you collect just by touching it.
/// Made for a silly joke level
/// Author: Sean Lee 3/1/2024
/// </summary>

public class FastMissionCollectable : MissionCollectable
{
    [SerializeField] CollectMissionBehavior mission;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            mission.Collected();
            gameObject.SetActive(false);
        }
    }
}
