using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NoClip Command", menuName = "Commands/NoClipCommand")]
public class NoClipCommand : ConsoleCommand
{
    [SerializeField] int playerLayerNum = 3;
    [SerializeField] int NoClipLayerNum = 20;
    public override (bool, string) ActivateCommand(string[] args)
    {
        switch (args.Length)
        {
            case 0:
                int layerValue = DeveloperConsole.instance.player.layer == NoClipLayerNum ? playerLayerNum : NoClipLayerNum;
                DeveloperConsole.instance.player.layer = layerValue;
                string message = DeveloperConsole.instance.player.layer == NoClipLayerNum ? " on" : " off";
                return (true, validCommandMessage + message);
            case 1:
                if (args[0].Equals("true", System.StringComparison.OrdinalIgnoreCase))
                {
                    DeveloperConsole.instance.player.layer = NoClipLayerNum;
                    return (true, validCommandMessage + " on");
                }
                else if (args[0].Equals("false", System.StringComparison.OrdinalIgnoreCase))
                {
                    DeveloperConsole.instance.player.layer = playerLayerNum;
                    return (true, validCommandMessage + " off");
                }
                break;
            default:
                return (false, "");

        }
        return (false, "");
    }
}
