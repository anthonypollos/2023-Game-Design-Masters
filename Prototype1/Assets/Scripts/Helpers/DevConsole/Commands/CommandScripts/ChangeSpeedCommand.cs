using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ChangeSpeed Command", menuName = "Commands/ChangeSpeedCommand")]
public class ChangeSpeedCommand : ConsoleCommand
{
    public override (bool, string) ActivateCommand(string[] args)
    {
        float arg;
        if (!float.TryParse(args[0], out arg))
        {
            return (false, "");
        }
        if (arg <= 0)
        {
            return (false, "");
        }
        DeveloperConsole.instance.playerController.ChangeSpeed(arg);
        return (true, validCommandMessage + arg);
    }
}

