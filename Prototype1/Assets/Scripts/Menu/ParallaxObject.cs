using System.Collections;
using UnityEngine;

public class ParallaxObject : MonoBehaviour
{
    private InputChecker inputChecker;

    /// <summary>
    /// 
    /// </summary>
    private Vector3 startPos;

    [SerializeField] [Tooltip("")] private float moveSens;

    /// <summary>
    /// 
    /// </summary>
    private float moveTime = 1.5f;

    void Start()
    {
        startPos = transform.position;

        inputChecker = FindObjectOfType<InputChecker>();
    }

    void Update()
    {
        if (inputChecker.IsController())
            ParallaxAxis();
        else
            ParallaxMouse();
    }

    /// <summary>
    /// 
    /// </summary>
    private void ParallaxMouse()
    {
        float newPosX = Camera.main.ScreenToViewportPoint(Input.mousePosition).x - 0.5f;
        float newPosY = Camera.main.ScreenToViewportPoint(Input.mousePosition).y - 0.5f;

        newPosX = Mathf.Clamp(newPosX, -0.5f, 0.5f);
        newPosY = Mathf.Clamp(newPosY, -0.5f, 0.5f);

        Parallax(newPosX, newPosY);
    }

    /// <summary>
    /// 
    /// </summary>
    private void ParallaxAxis()
    {
        float newPosX = Input.GetAxis("Horizontal");// / 2;
        float newPosY = Input.GetAxis("Vertical");// / 2;

        Parallax(newPosX, newPosY);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    private void Parallax(float posX, float posY)
    {
        float targetPosX = Mathf.Lerp(transform.position.x, startPos.x + (posX * moveSens), moveTime * Time.deltaTime);
        float targetPosY = Mathf.Lerp(transform.position.y, startPos.y + (posY * moveSens), moveTime * Time.deltaTime);

        transform.position = new Vector3(targetPosX, targetPosY, startPos.z);
    }
}
