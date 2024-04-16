using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTemp : MonoBehaviour
{
    public Animator anim;

    public bool isFuse;

    // Start is called before the first frame update
    void Start()
    {
        anim.SetBool("isCutscene", false);

        //if (isFuse)
          //  anim.SetBool("isFuse", true);

        //if (!isFuse)
          //  anim.SetBool("isFuse", false);
    }
}
