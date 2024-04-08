using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueButtonTemp : MonoBehaviour, ISaveable
{
    [SerializeField] Button continueButton;

    public void LoadData(SavedValues savedValues)
    {
        continueButton.interactable = savedValues.levels.ContainsKey("HubScene");
    }

    public void SaveData(ref SavedValues savedValues)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
