using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New SpawnEntity Command", menuName = "Commands/SpawnEntityCommand")]
public class SpawnEntityCommand : ConsoleCommand
{

    [SerializeField] List<GameObject> spawnablePrefabs;
    [SerializeField] float unitsToSpawnInFrontOfPlayer = 3;
    List<string> spawnableNames;

    public override (bool, string) ActivateCommand(string[] args)
    {
        if(spawnableNames == null)
        {
            spawnableNames = new List<string>();
        }
        if (true) 
        {
            //Debug.Log("setting spawnable names");
            spawnableNames = new List<string>();
            spawnableNames.Clear();
            foreach(GameObject pref in spawnablePrefabs)
            {
                if (pref==null)
                {
                    spawnableNames.Add(string.Empty);
                    continue;
                }
                spawnableNames.Add(pref.name);
                //Debug.Log(pref.name + " added");
            }
        }
        if(args.Length == 1)
        {
            bool temp = false;
            int index = -1;
            for(int i = 0; i < spawnableNames.Count; i++)
            {
                if (spawnableNames[i] == string.Empty)
                {
                    continue;
                }
                if (string.Equals(args[0], spawnableNames[i], StringComparison.OrdinalIgnoreCase))
                {
                    temp = true;
                    index = i;
                    break;
                }
            }
            if(temp)
            {
                //Debug.Log("Index set to: " +  index);
                GameObject player = DeveloperConsole.instance.player;
                Vector3 spawnPos = player.transform.position + player.transform.forward * unitsToSpawnInFrontOfPlayer;
                Instantiate(spawnablePrefabs[index], spawnPos, Quaternion.identity);
                return (temp, spawnableNames[index] + " " + validCommandMessage);
            }
            //Debug.Log("name not found");
            return (temp, "Error has occured");
        }
        else
        {
            return (false, "");
        }
    }

}
