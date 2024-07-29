using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AccessibilityOptions : MonoBehaviour
{
    [Header("Cursor Variables")]
    [SerializeField] private RectTransform cursor;

    [SerializeField] private TextMeshProUGUI cursorScaleText;
    [SerializeField] private Slider cursorScaleSlider;

    /// <summary>
    /// Custom Cursor Sprites
    /// </summary>
    [SerializeField] private Image cursorOuter, cursorInner;

    /// <summary>
    /// Cursor Color panel display cursor
    /// </summary>
    [SerializeField] private Image cursorOuterUI, cursorInnerUI;

    [SerializeField] private FlexibleColorPicker outerPicker, innerPicker;

    private Color outerColor, innerColor;

    [Header("Outline Variables")]
    [SerializeField] private TextMeshProUGUI outlineWidthText;
    [SerializeField] private Slider outlineWidthSlider;

    [SerializeField] private Color defaultOutlineColor;
    private Color playerColor, enemyColor, throwableColor,
                    hazardColor, collectColor, healthColor, npcColor;
    [SerializeField] private FlexibleColorPicker playerPicker, enemyPicker, throwablePicker,
                    hazardPicker, collectPicker, healthPicker, npcPicker;

    [SerializeField] private FlexibleColorPicker[] pickers;

    private void Awake()
    {

    }

    #region Custom Cursor
    #region Cursor Scale
    /// <summary>
    /// Get target cursor scale value
    /// </summary>
    /// <param name="mod"></param>
    public void AdjustCursorScale(int mod)
    {
        int val = (int)cursorScaleSlider.value + mod;

        switch(val)
        {
            case (9):
                break;
            case (21):
                break;
            default:
                SetCursorScale(val);
                break;
        }
    }

    /// <summary>
    /// Set the target cursor scale
    /// </summary>
    /// <param name="value">Cursor scale modifier</param>
    public void SetCursorScale(float value)
    {
        PlayerPrefs.SetFloat("CursorScale", value);

        cursorScaleSlider.value = value;

        value /= 10;

        cursor.localScale = new Vector3(value, value, value);

        cursorScaleText.text = (value * 100) + "%";
    }
    #endregion

    #region Cursor Color
    public void SetCursorColor(Color innerColor, Color outerColor)
    {
        this.outerColor = outerColor;
        cursorOuter.color = outerColor;
        cursorOuterUI.color = outerColor;
        outerPicker.SetColorNoAlpha(outerColor);
        outerPicker.gameObject.SetActive(false);
        outerPicker.gameObject.SetActive(true);

        this.innerColor = innerColor;
        cursorInner.color = innerColor;
        cursorInnerUI.color = innerColor;
        innerPicker.SetColorNoAlpha(innerColor);
        innerPicker.gameObject.SetActive(false);
        innerPicker.gameObject.SetActive(true);
    }

    public void SetCursorColor()
    {
        outerPicker.color = outerColor;
        innerPicker.color = innerColor;
    }

    public void SetOuterColor(Color color)
    {
        outerColor = color;

        float outerR = outerColor.r;
        float outerG = outerColor.g;
        float outerB = outerColor.b;

        PlayerPrefs.SetFloat("OuterCursorR", outerR);
        PlayerPrefs.SetFloat("OuterCursorG", outerG);
        PlayerPrefs.SetFloat("OuterCursorB", outerB);

        cursorOuter.color = outerColor;

        cursorOuterUI.color = outerColor;
    }
    
    public void SetInnerColor(Color color)
    {
        innerColor = color;

        float innerR = innerColor.r;
        float innerG = innerColor.g;
        float innerB = innerColor.b;

        PlayerPrefs.SetFloat("InnerCursorR", innerR);
        PlayerPrefs.SetFloat("InnerCursorG", innerG);
        PlayerPrefs.SetFloat("InnerCursorB", innerB);

        cursorInner.color = innerColor;

        cursorInnerUI.color = innerColor;
    }
    #endregion
    #endregion

    #region Custom Outlines
    #region Outline Width
    public void AdjustOutlineWidth(int mod)
    {
        int val = (int)outlineWidthSlider.value + mod;

        switch (val)
        {
            case (9):
                break;
            case (21):
                break;
            default:
                SetOutlineWidth(val);
                break;
        }
    }

    public void SetOutlineWidth(float value)
    {
        PlayerPrefs.SetFloat("OutlineWidth", value);

        outlineWidthSlider.value = value;

        value /= 10;

        outlineWidthText.text = ((value - 0.5f) * 100) + "%";

        OutlineCustomizer[] outlines = FindObjectsOfType<OutlineCustomizer>();

        if(outlines !=  null)
        {
            foreach (OutlineCustomizer outline in outlines)
            {
                outline.UpdateOutlines();
            }
        }
    }
    #endregion

    #region Outline Color
    public void UpdateOutlineColors()
    {
        OutlineCustomizer[] outlines = FindObjectsOfType<OutlineCustomizer>();

        if (outlines != null)
        {
            foreach (OutlineCustomizer outline in outlines)
            {
                outline.UpdateOutlines();
            }
        }
    }

    public void SetOutlineColors()
    {
        float enemyColorR = PlayerPrefs.GetFloat("EnemyOutlineR", defaultOutlineColor.r);
        float enemyColorG = PlayerPrefs.GetFloat("EnemyOutlineG", defaultOutlineColor.g);
        float enemyColorB = PlayerPrefs.GetFloat("EnemyOutlineB", defaultOutlineColor.b);

        enemyColor = new Color(enemyColorR, enemyColorG, enemyColorB);

        float throwableColorR = PlayerPrefs.GetFloat("ThrowableOutlineR", defaultOutlineColor.r);
        float throwableColorG = PlayerPrefs.GetFloat("ThrowableOutlineG", defaultOutlineColor.g);
        float throwableColorB = PlayerPrefs.GetFloat("ThrowableOutlineB", defaultOutlineColor.b);

        throwableColor = new Color(throwableColorR, throwableColorG, throwableColorB);

        float hazardColorR = PlayerPrefs.GetFloat("HazardOutlineR", defaultOutlineColor.r);
        float hazardColorG = PlayerPrefs.GetFloat("HazardOutlineG", defaultOutlineColor.g);
        float hazardColorB = PlayerPrefs.GetFloat("HazardOutlineB", defaultOutlineColor.b);

        hazardColor = new Color(hazardColorR, hazardColorG, hazardColorB);

        float collectColorR = PlayerPrefs.GetFloat("CollectOutlineR", defaultOutlineColor.r);
        float collectColorG = PlayerPrefs.GetFloat("CollectOutlineG", defaultOutlineColor.g);
        float collectColorB = PlayerPrefs.GetFloat("CollectOutlineB", defaultOutlineColor.b);

        collectColor = new Color(collectColorR, collectColorG, collectColorB);

        float healthColorR = PlayerPrefs.GetFloat("HealthOutlineR", defaultOutlineColor.r);
        float healthColorG = PlayerPrefs.GetFloat("HealthOutlineG", defaultOutlineColor.g);
        float healthColorB = PlayerPrefs.GetFloat("HealthOutlineB", defaultOutlineColor.b);

        healthColor = new Color(healthColorR, healthColorG, healthColorB);

        float playerColorR = PlayerPrefs.GetFloat("PlayerOutlineR", defaultOutlineColor.r);
        float playerColorG = PlayerPrefs.GetFloat("PlayerOutlineG", defaultOutlineColor.g);
        float playerColorB = PlayerPrefs.GetFloat("PlayerOutlineB", defaultOutlineColor.b);

        playerColor = new Color(playerColorR, playerColorG, playerColorB);

        float npcColorR = PlayerPrefs.GetFloat("NPCOutlineR", defaultOutlineColor.r);
        float npcColorG = PlayerPrefs.GetFloat("NPCOutlineG", defaultOutlineColor.g);
        float npcColorB = PlayerPrefs.GetFloat("NPCOutlineB", defaultOutlineColor.b);

        npcColor = new Color(npcColorR, npcColorG, npcColorB);
    }

    public void SetOutlineSliderColors()
    {
        playerPicker.SetColorNoAlpha(playerColor);
        enemyPicker.SetColorNoAlpha(enemyColor);
        throwablePicker.SetColorNoAlpha(throwableColor);
        hazardPicker.SetColorNoAlpha(hazardColor);
        collectPicker.SetColorNoAlpha(collectColor);
        healthPicker.SetColorNoAlpha(healthColor);
        npcPicker.SetColorNoAlpha(npcColor);

        foreach (FlexibleColorPicker picker in pickers)
        {
            picker.enabled = false;
            picker.enabled = true;
        }
    }

    public void ResetSliders()
    {
        SetPlayerColor(defaultOutlineColor);
        SetEnemyColor(defaultOutlineColor);
        SetThrowableColor(defaultOutlineColor);
        SetHazardColor(defaultOutlineColor);
        SetCollectColor(defaultOutlineColor);
        SetHealthColor(defaultOutlineColor);
        SetNPCColor(defaultOutlineColor);

        SetOutlineSliderColors();
    }

    public void SetOutlineColor(Color color, string type)
    {
        PlayerPrefs.SetFloat(type + "R", color.r);
        PlayerPrefs.SetFloat(type + "G", color.g);
        PlayerPrefs.SetFloat(type + "B", color.b);
    }

    public void SetPlayerColor(Color color)
    {
        playerColor = color;
        SetOutlineColor(color, "PlayerOutline");
    }

    public void SetEnemyColor(Color color)
    {
        enemyColor = color;
        SetOutlineColor(color, "EnemyOutline");
    }

    public void SetThrowableColor(Color color)
    {
        throwableColor = color;
        SetOutlineColor(color, "ThrowableOutline");
    }

    public void SetHazardColor(Color color)
    {
        hazardColor = color;
        SetOutlineColor(color, "HazardOutline");
    }

    public void SetCollectColor(Color color)
    {
        collectColor = color;
        SetOutlineColor(color, "CollectOutline");
    }

    public void SetHealthColor(Color color)
    {
        healthColor = color;
        SetOutlineColor(color, "HealthOutline");
    }

    public void SetNPCColor(Color color)
    {
        npcColor = color;
        SetOutlineColor(color, "NPCOutline");
    }

    #endregion
    #endregion
}
