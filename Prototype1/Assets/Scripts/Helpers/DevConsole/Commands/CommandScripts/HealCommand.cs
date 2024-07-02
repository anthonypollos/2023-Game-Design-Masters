using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Command", menuName = "Commands/HealCommand")]
public class HealCommand : ConsoleCommand
{
    public override (bool, string) ActivateCommand(string[] args)
    {
        DeveloperConsole.instance.playerHealth.TakeDamage(-99999999);
        return (true, validCommandMessage);
    }
}
