using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/*
 * 
 * Purpose: Change the color of an object without modifying the material directly
 * Author: Sean Lee
 * Date: 11/28/23
 * 
 * NOTE: YOU NEED TO DISABLE AND RE-ENABLE THE OBJECT IN-EDITOR EVERY TIME YOU CHANGE THE COLOR FOR IT TO BE VISUALIZED!
 * THIS IT INCREDIBLY FUCKING STUPID BUT I CAN'T FIND A METHOD THAT LETS ME UPDATE IT EVERY TIME THE COLOR CHANGES!
 * 
 */

[ExecuteInEditMode]
public class ObjectColor : MonoBehaviour
{
    [Tooltip("OPAQUE ONLY!\nThis boolean makes the object glow.")]
    public bool _emissiveObject = false;
    [Tooltip("Make the object unlit.\nThis should really only be used for translucent materials that we want to glow, but should be fine for anything, theoretically.")]
    public bool _unlit = false;
    public Color _myColor = new Color(255, 255, 255, 255);
    [Tooltip("OPAQUE ONLY!\nIf you want the object to be emissive this is the color it will emit.")]
    public Color _emissiveColor = new Color(0, 0, 0, 0);
    [Tooltip("The number (Found under Mesh Renderer's ''Material'' section) of the Material we want to color.")]
    public int MaterialNumber = 0;
    public float UScale = 1f;
    public float VScale = 1f;
    public float UOffset = 0f;
    public float VOffset = 0f;

    //set the color
    private void OnEnable()
    {
        ColorUpdate();
    }
    private void ColorUpdate()
    {
        //Does this object have only one material? If so, change it
        //NOTE: I thought arrays started at zero in C# but setting this to be less than 1 resulted in 
        if (GetComponent<MeshRenderer>().sharedMaterials.Length < 2)
        {
            //This extra stuff is to keep Unity from yelling at me in the editor.
            var tempMaterial = new Material(GetComponent<MeshRenderer>().sharedMaterial);
            tempMaterial.color = _myColor;
            if (_emissiveObject)
            {
                //This enables Emission on the material.
                tempMaterial.EnableKeyword("_EMISSION");
                //This sets the Emission color to our serialized color.
                tempMaterial.SetColor("_EmissionColor", _emissiveColor);
                //This sets the Emission map to our material's texture.
                tempMaterial.SetTexture("_EmissionMap", tempMaterial.GetTexture("_BaseMap"));
            }
            else
            {
                tempMaterial.DisableKeyword("_EMISSION");
                tempMaterial.SetTexture("_EmissionMap", null);
            }
            if (_unlit)
            {
                tempMaterial.shader = Shader.Find("Universal Render Pipeline/Unlit");
            }
            else
            {
                tempMaterial.shader = Shader.Find("Universal Render Pipeline/Lit");
            }

            tempMaterial.mainTextureScale = new Vector2(UScale, VScale);
            tempMaterial.mainTextureOffset = new Vector2(UOffset, VOffset);

            GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
            //Debug text. Seems to work
            //print("Object " + this.name + " has been detected as only having one material.");
        }
        //otherwise, pull "MaterialNumber" and use that as the material to replace
        else
        {
            var tempMaterial = new Material(GetComponent<MeshRenderer>().sharedMaterials[MaterialNumber]);
            tempMaterial.color = _myColor;
            if (_emissiveObject)
            {
                //This enables Emission on the material.
                tempMaterial.EnableKeyword("_EMISSION");
                //This sets the Emission color to our serialized color.
                tempMaterial.SetColor("_EmissionColor", _emissiveColor);
                //This sets the Emission map to our material's texture.
                tempMaterial.SetTexture("_EmissionMap", tempMaterial.GetTexture("_BaseMap")); 
            }
            else
            {
                tempMaterial.DisableKeyword("_EMISSION");
                tempMaterial.SetTexture("_EmissionMap", null);
            }
            if (_unlit)
            {
                tempMaterial.shader = Shader.Find("Universal Render Pipeline/Unlit");
            }
            else
            {
                tempMaterial.shader = Shader.Find("Universal Render Pipeline/Lit");
            }
            GetComponent<MeshRenderer>().sharedMaterials[MaterialNumber] = tempMaterial;

            //Debug text. Seems to work
            //print("Object " + this.name + " has been detected as having "+ GetComponent<MeshRenderer>().sharedMaterials.Length + " materials.");
        }
    }
}
