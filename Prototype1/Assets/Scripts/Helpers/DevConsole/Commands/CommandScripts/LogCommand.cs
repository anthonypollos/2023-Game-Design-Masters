using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Log Command", menuName = "Commands/LogCommand")]
public class LogCommand : ConsoleCommand
{
    public override (bool, string) ActivateCommand(string[] args)
    {
        Debug.Log(string.Join(" ", args) + "\nTotal Args: " + args.Length);
        return (true, validCommandMessage);
    }
}
