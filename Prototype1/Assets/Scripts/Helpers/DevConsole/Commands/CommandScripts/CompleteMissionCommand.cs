using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New GodMode Command", menuName = "Commands/CompleteMissionCommand")]
public class CompleteMissionCommand : ConsoleCommand
{
    public override (bool, string) ActivateCommand(string[] args)
    {
        int arg;
        if (!int.TryParse(args[0], out arg))
        {
            return (false, "");
        }
        bool temp = DeveloperConsole.instance.missionFolder.SetComplete(arg);
        return (temp, validCommandMessage);
    }
}
