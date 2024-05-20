using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha0))
            Time.timeScale = 0.25f;

        if (Input.GetKey(KeyCode.Alpha9))
            Time.timeScale = 0.75f;

        if (Input.GetKey(KeyCode.Alpha8))
            Time.timeScale = 1;
    }
}
