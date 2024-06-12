using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class VictoryPortal : InteractableBehaviorTemplate
{
    [SerializeField] string worldName;

    public override bool Interact()
    {
        DeveloperConsole.instance.missionFolder.Win();
        //SceneManager.LoadScene("HubScene");
        SceneLoader.Instance.LoadScene(worldName);
        return false;
    }
}
