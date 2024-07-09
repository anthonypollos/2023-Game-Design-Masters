using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CutsceneManager : MonoBehaviour
{
    public string sceneToLoad;
    MainControls mainControls;

    // Start is called before the first frame update
    void OnEnable()
    {
        mainControls = ControlsContainer.instance.mainControls;
        mainControls.Main.Interact.performed += Interact;
    }

    private void OnDisable()
    {
        mainControls.Main.Interact.performed -= Interact;
    }

    private void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            Skip();
    }

    // Update is called once per frame
    void Skip()
    {
        SceneLoader.Instance.LoadScene(sceneToLoad);
    }

    // level progression instance check, move things to one scene instead of 5
}
