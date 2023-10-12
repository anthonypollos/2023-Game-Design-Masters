using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class HubPortal : InteractableBehaviorTemplate
{
    [SerializeField] string worldName;
    public override bool Interact()
    {
        SceneManager.LoadScene(worldName);
        return false;
    }

}
