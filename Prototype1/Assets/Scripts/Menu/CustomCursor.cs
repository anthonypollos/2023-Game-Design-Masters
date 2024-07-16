using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [SerializeField] private Vector2 spriteOffset;

    [SerializeField] private Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCursor();

        if (Cursor.visible != false)
            Cursor.visible = false;
    }

    private void MoveCursor()
    {
        Vector2 cursorPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out cursorPos);
        transform.position = canvas.transform.TransformPoint(cursorPos + spriteOffset);
        //transform.position = Camera.main.ScreenToWorldPoint(cursorPos);// + spriteOffset); //new Vector3(cursorPos.x + spriteOffset.x, cursorPos.y + spriteOffset.y, -10);
    }
}