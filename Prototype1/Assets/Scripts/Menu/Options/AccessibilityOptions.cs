using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AccessibilityOptions : MonoBehaviour
{
    [Header("Cursor Variables")]
    [SerializeField] private RectTransform cursor;
    [SerializeField] TextMeshProUGUI cursorScaleText;
    [SerializeField] private Image cursorOuter;
    [SerializeField] private Image cursorInner;

    private Vector3 cursorOuterColor;
    private Vector3 cursorInnerColor;

    #region Custom Cursor
    /// <summary>
    /// Set the target cursor scale
    /// </summary>
    /// <param name="value">Cursor scale modifier</param>
    public void SetCursorScale(float value)
    {
        PlayerPrefs.SetFloat("CursorScale", value);

        cursor.localScale = new Vector3(value, value, value);

        cursorScaleText.text = (value * 100) + "%";
    }

    public void SetOuterCursorColor(float value)
    {
        SetCursorColor(value);
    }

    public void SetInnerCursorColor(float value)
    {
        SetCursorColor(value);
    }

    private void SetCursorColor(float value) // variable for RGB?
    {

    }

    #endregion
}
