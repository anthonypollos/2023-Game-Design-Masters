using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GodMode Command", menuName = "Commands/GodModeCommand")]
public class GodModeCommand : ConsoleCommand
{
    public override (bool, string) ActivateCommand(string[] args)
    {
        switch (args.Length)
        {
            case 0:
                DeveloperConsole.instance.godMode = !DeveloperConsole.instance.godMode;
                string message = DeveloperConsole.instance.godMode ? " on" : " off";
                return (true, validCommandMessage + message);
            case 1:
                if(args[0].Equals("true", System.StringComparison.OrdinalIgnoreCase))
                {
                    DeveloperConsole.instance.godMode = true;
                    return (true, validCommandMessage + " on");
                }
                else if (args[0].Equals("false", System.StringComparison.OrdinalIgnoreCase))
                {
                    DeveloperConsole.instance.godMode = false;
                    return (true, validCommandMessage + " off");
                }
                break;
            default:
                return (false, "");

        }
        return (false, "");
    }
}
