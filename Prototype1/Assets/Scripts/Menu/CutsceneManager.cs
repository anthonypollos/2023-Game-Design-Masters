using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public string sceneToLoad;
    MainControls mainControls;

    private bool canSkip = true;

    [SerializeField] List<string> cutsceneScenes;

    // Start is called before the first frame update
    void OnEnable()
    {
        mainControls = ControlsContainer.instance.mainControls;
        mainControls.Main.Interact.performed += _ => Interact();
    }

    private void Interact()
    {
        if(cutsceneScenes.Contains(SceneManager.GetActiveScene().name) && canSkip)
        {
            canSkip = false;
            Skip();
        }
        //if (ctx.performed)
        //Skip();
    }

    // Update is called once per frame
    void Skip()
    {
        SceneLoader.Instance.LoadScene(sceneToLoad);
    }

    // level progression instance check, move things to one scene instead of 5
}
