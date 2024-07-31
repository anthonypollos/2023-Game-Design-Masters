using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

//Script by One Wheel Studio here https://youtu.be/TD0R5x0yL0Y?si=8uKwY1sPx7yUnuVs
public class RebindUI : MonoBehaviour
{
    [SerializeField]
    private InputActionReference inputActionReference;
    [SerializeField]
    private bool excludeMouse = true;
    [Range(0, 10)] [SerializeField]
    private int selectBinding;
    [SerializeField]
    private InputBinding.DisplayStringOptions displayStringOptions;
    [Header("Binding Info - DO NOT EDIT")]
    [SerializeField]
    private InputBinding inputBinding;
    private int bindingIndex;

    private string actionName;

    [Header("UI Fields")]
    [SerializeField]
    private TextMeshProUGUI actionText;
    [SerializeField]
    private Button rebindButton;
    [SerializeField]
    private TextMeshProUGUI rebindText;
    [SerializeField]
    private Button resetButton;
    [SerializeField]
    private Button resetAllButton;


    private void OnEnable()
    {
        rebindButton.onClick.AddListener(() => DoRebind());
        resetButton.onClick.AddListener(() => ResetBinding());
        resetAllButton.onClick.AddListener(() => ResetBinding());

        if(inputActionReference != null)
        {
            GetBindingInfo();
            UpdateUI();
        }

        ControlsContainer.instance.rebindComplete += UpdateUI;
        ControlsContainer.instance.rebindCanceled += UpdateUI;
    }

    private void OnDisable()
    {
        ControlsContainer.instance.rebindComplete -= UpdateUI;
        ControlsContainer.instance.rebindComplete -= UpdateUI;
    }


    private void OnValidate()
    {
        if (inputActionReference == null)
            return;
        GetBindingInfo();
        UpdateUI();
    }

    private void GetBindingInfo()
    {
        if(inputActionReference!=null)
        {
            actionName = inputActionReference.action.name;

            if(inputActionReference.action.bindings.Count > selectBinding)
            {
                inputBinding = inputActionReference.action.bindings[selectBinding];
                bindingIndex = selectBinding;
            }
        }


    }

    private void UpdateUI()
    {
        if (actionText != null)
            actionText.text = actionName;

        if(rebindText != null)
        {
            if (Application.isPlaying)
            {
                rebindText.text = ControlsContainer.instance.GetBindingName(actionName, bindingIndex, displayStringOptions).TranslateToSprite();
            }
            else
            {
                rebindText.text = inputActionReference.action.GetBindingDisplayString(bindingIndex, displayStringOptions).TranslateToSprite();
            }
        }
    }

    private void DoRebind()
    {
        ControlsContainer.instance.StartRebind(actionName, bindingIndex, rebindText);
    }

    private void ResetBinding()
    {
        //Debug.Log("Resetting");
        ControlsContainer.instance.ResetBinding(actionName, bindingIndex);
        UpdateUI();
    }
}



