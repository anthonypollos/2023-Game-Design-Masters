using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimLineToggle : MonoBehaviour
{
    // THIS IS TEMP.

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Gradient mouseColor, controllerColor;

    private InputChecker inputChecker;

    private bool isController = false;

    // Start is called before the first frame update
    void Start()
    {
        inputChecker = FindObjectOfType<InputChecker>();
        lineRenderer = GetComponent<LineRenderer>();

        SetMouse();
    }

    // Update is called once per frame
    void Update()
    {
        if (isController != inputChecker.IsController())
        {
            isController = inputChecker.IsController();

            if (isController)
                SetController();
            else if (!isController)
                SetMouse();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void SetMouse()
    {
        lineRenderer.colorGradient = mouseColor;
    }

    /// <summary>
    /// 
    /// </summary>
    private void SetController()
    {
        lineRenderer.colorGradient = controllerColor;
    }
}
