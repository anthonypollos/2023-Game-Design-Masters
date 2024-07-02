using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Change Scene Command", menuName = "Commands/SceneCommand")]
public class ChangeSceenCommand : ConsoleCommand
{
    public override (bool, string) ActivateCommand(string[] args)
    {
        string sceneName = args[0];
        SceneManager.LoadScene(sceneName);
        if (!SceneManager.GetSceneByName(sceneName).IsValid())
            return (false, "");
        else
            return (true, validCommandMessage);
    }
}
