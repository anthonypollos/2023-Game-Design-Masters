using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System;

public class ControlsContainer : MonoBehaviour, ISaveable
{
    public static ControlsContainer instance;


    public MainControls mainControls {get; private set; }
    public event Action rebindComplete;
    public event Action rebindCanceled;
    public event Action<InputAction, int> rebindStarted;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            transform.parent = null;
            DontDestroyOnLoad(this.gameObject);
            if(mainControls == null)
                mainControls = new MainControls();
            mainControls.Enable();
        }
        else
        {
            Destroy(this);
        }

        LoadBindings();
    }

    private void OnEnable()
    {
        if (mainControls != null)
        {
            mainControls.Enable();
        }

    }

    private void OnDisable()
    {
        if(mainControls != null)
        {
            mainControls.Disable();
        }
    }

    public void SaveData(ref SavedValues savedValues)
    {

    }

    public void LoadData(SavedValues savedValues)
    {

    }

    //Script by One Wheel Studio here https://youtu.be/TD0R5x0yL0Y?si=8uKwY1sPx7yUnuVs

    public void StartRebind(string actionName, int bindingIndex, TextMeshProUGUI statusText)
    {
        InputAction action = mainControls.asset.FindAction(actionName);
        if(action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Couldn't find action or binding");
            return;
        }

        if(action.bindings[bindingIndex].isComposite)
        {
            //Debug.Log("Composite Time");
            var firstPartIndex = bindingIndex + 1;
            //Debug.Log($"{firstPartIndex} < {action.bindings.Count} && {action.bindings[firstPartIndex].isPartOfComposite}");
            if(firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
            {
                //Debug.Log("Composite in");
                DoRebind(action, firstPartIndex, statusText, true);
            }
        }
        else
        {
            DoRebind(action, bindingIndex, statusText, false);
        }

    }
    private void DoRebind(InputAction actionToRebind, int bindingIndex, TextMeshProUGUI statusText, bool allCompositeParts)
    {
        if (actionToRebind == null || bindingIndex < 0)
            return;

        //Rebind Feedback
        statusText.text = $"Press a {actionToRebind.expectedControlType}";

        actionToRebind.Disable();

        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);

        rebind.OnComplete(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();

            if(allCompositeParts)
            {
                var nextBindingIndex = bindingIndex + 1;
                if (nextBindingIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingIndex].isPartOfComposite)
                {
                    DoRebind(actionToRebind, nextBindingIndex, statusText, allCompositeParts);
                }
            }
            SaveBindingOverride(actionToRebind);
            rebindComplete?.Invoke();
        });

        rebind.OnCancel(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();

            rebindCanceled?.Invoke();
        });

        rebind.WithCancelingThrough("<Keyboard>/escape");
        rebind.WithCancelingThrough("<Gamepad>/start");
        rebind.WithCancelingThrough("<Keyboard>/p");
        rebind.WithControlsExcluding("<Gamepad>/leftstick");
        rebind.WithControlsExcluding("<Gamepad>/rightstick");

        rebindStarted?.Invoke(actionToRebind, bindingIndex);
        rebind.Start();
    }

    public string GetBindingName(string actionName, int bindingIndex, InputBinding.DisplayStringOptions displayStringOptions)
    {
        if (mainControls == null)
            mainControls = new MainControls();

        InputAction action = mainControls.asset.FindAction(actionName);
        return action.GetBindingDisplayString(bindingIndex, displayStringOptions);
    }

    private void SaveBindingOverride(InputAction action)
    {
        for(int i = 0; i< action.bindings.Count; i++)
        {
            PlayerPrefs.SetString(action.actionMap + action.name + i, action.bindings[i].overridePath);
        }
    }

    private void LoadBindings()
    {
        foreach (InputAction action in mainControls)
        {
            LoadBindingOverride(action);
        }
    }

    private void LoadBindingOverride(InputAction action)
    {
        if (mainControls == null)
            mainControls = new MainControls();

        for(int i = 0; i< action.bindings.Count; i++)
        {
            string overridePath = PlayerPrefs.GetString(action.actionMap + action.name + i);
            if (!string.IsNullOrEmpty(overridePath))
            {
                action.ApplyBindingOverride(i, overridePath);
            }
        }
        rebindComplete?.Invoke();
    }

    public void ResetBinding(string actionName, int bindingIndex)
    {

        Debug.Log("resetting");
        InputAction action = mainControls.asset.FindAction(actionName);

        if(action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Could not find action or binding");
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            for (int i = bindingIndex; i < action.bindings.Count && (action.bindings[i].isPartOfComposite || action.bindings[i].isComposite); i++)
            {
                action.RemoveBindingOverride(i);
                PlayerPrefs.SetString(action.actionMap + action.name + i, String.Empty);
            }
        }
        else
        {
            action.RemoveBindingOverride(bindingIndex);
            PlayerPrefs.SetString(action.actionMap + action.name + bindingIndex, String.Empty);
        }
    }


}
