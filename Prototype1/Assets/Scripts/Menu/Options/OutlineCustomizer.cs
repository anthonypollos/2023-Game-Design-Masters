using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineCustomizer : MonoBehaviour
{
    public enum ObjectType { Enemy, Throwable, Hazard, Collectible, Health, Player, NPC }

    [SerializeField] ObjectType objectType;

    [SerializeField] private Color defaultOutlineColor;
    [SerializeField] private float outlineWidthMod = 1f;

    private Outline outline;

    private void Awake()
    {
        Invoke("EnableOutlines", 0.05f);
    }

    private void EnableOutlines()
    {
        outline = GetComponent<Outline>();

        if (outline != null)
        {
        outline.enabled = true;

        outline.OutlineMode = Outline.Mode.OutlineVisible;

        UpdateOutlines();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateOutlines()
    {
        float width = PlayerPrefs.GetFloat("OutlineWidth", 17f);

        if(outline != null)
            outline.OutlineWidth = (width/10) * outlineWidthMod;

        Color outlineColor = defaultOutlineColor;
        outlineColor.a = 1f;

        switch(objectType)
        {
            case ObjectType.Enemy:
                outlineColor.r = PlayerPrefs.GetFloat("EnemyOutlineR", defaultOutlineColor.r);
                outlineColor.g = PlayerPrefs.GetFloat("EnemyOutlineG", defaultOutlineColor.g);
                outlineColor.b = PlayerPrefs.GetFloat("EnemyOutlineB", defaultOutlineColor.b);
                break;
            case ObjectType.Throwable:
                outlineColor.r = PlayerPrefs.GetFloat("ThrowableOutlineR", defaultOutlineColor.r);
                outlineColor.g = PlayerPrefs.GetFloat("ThrowableOutlineG", defaultOutlineColor.g);
                outlineColor.b = PlayerPrefs.GetFloat("ThrowableOutlineB", defaultOutlineColor.b);
                break;
            case ObjectType.Hazard:
                outlineColor.r = PlayerPrefs.GetFloat("HazardOutlineR", defaultOutlineColor.r);
                outlineColor.g = PlayerPrefs.GetFloat("HazardOutlineG", defaultOutlineColor.g);
                outlineColor.b = PlayerPrefs.GetFloat("HazardOutlineB", defaultOutlineColor.b);
                break;
            case ObjectType.Collectible:
                outlineColor.r = PlayerPrefs.GetFloat("CollectOutlineR", defaultOutlineColor.r);
                outlineColor.g = PlayerPrefs.GetFloat("CollectOutlineG", defaultOutlineColor.g);
                outlineColor.b = PlayerPrefs.GetFloat("CollectOutlineB", defaultOutlineColor.b);
                break;
            case ObjectType.Health:
                outlineColor.r = PlayerPrefs.GetFloat("HealthOutlineR", defaultOutlineColor.r);
                outlineColor.g = PlayerPrefs.GetFloat("HealthOutlineG", defaultOutlineColor.g);
                outlineColor.b = PlayerPrefs.GetFloat("HealthOutlineB", defaultOutlineColor.b);
                break;
            case ObjectType.Player:
                outlineColor.r = PlayerPrefs.GetFloat("PlayerOutlineR", defaultOutlineColor.r);
                outlineColor.g = PlayerPrefs.GetFloat("PlayerOutlineG", defaultOutlineColor.g);
                outlineColor.b = PlayerPrefs.GetFloat("PlayerOutlineB", defaultOutlineColor.b);
                break;
            case ObjectType.NPC:
                outlineColor.r = PlayerPrefs.GetFloat("NPCOutlineR", defaultOutlineColor.r);
                outlineColor.g = PlayerPrefs.GetFloat("NPCOutlineG", defaultOutlineColor.g);
                outlineColor.b = PlayerPrefs.GetFloat("NPCOutlineB", defaultOutlineColor.b);
                break;
        }

        if(outline != null)
            outline.OutlineColor = outlineColor;
    }
}
