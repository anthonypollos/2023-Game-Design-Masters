using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] fuseObjects, vipObjects;

    private bool isFuse;

    // Start is called before the first frame update
    void Start()
    {
        int fuseBool = PlayerPrefs.GetInt("IsFuse", 1);

        if (fuseBool == 1)
            isFuse = true;
        else
            isFuse = false;

        SetFuse(isFuse);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ToggleFuse();
    }


    private void ToggleFuse()
    {
        isFuse = !isFuse;

        if (isFuse == true)
            PlayerPrefs.SetInt("IsFuse", 1);
        else if (isFuse == false)
            PlayerPrefs.SetInt("IsFuse", 0);

        SetFuse(isFuse);
    }


    private void SetFuse(bool fuse)
    {
        foreach (GameObject obj in fuseObjects)
            obj.SetActive(fuse);

        foreach (GameObject obj in vipObjects)
            obj.SetActive(!fuse);

    }
}