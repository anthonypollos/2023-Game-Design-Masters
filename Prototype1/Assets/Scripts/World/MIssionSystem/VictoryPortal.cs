using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class VictoryPortal : InteractableBehaviorTemplate
{
    public override bool Interact()
    {
        DeveloperConsole.instance.missionFolder.Win();
        SceneManager.LoadScene("HubScene");
        return false;
    }
}
