using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public MenuBehavior mb;
    public string sceneToLoad;
    MainControls mainControls;

    // Start is called before the first frame update
    void OnEnable()
    {
        mainControls = new MainControls();
        mainControls.Main.Interact.Enable();
        mainControls.Main.Interact.performed += _ => Skip();

    }

    // Update is called once per frame
    void Skip()
    {
        mb.LoadScene(sceneToLoad);
    }
}
