using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSlotTemp : MonoBehaviour
{
    [SerializeField] TMP_Dropdown dropdown;
    // Start is called before the first frame update
    void Start()
    {
        dropdown.value = PlayerPrefs.GetInt("SaveSlot", 0);
    }

    public void SaveSlotUpdated(int slot)
    {
        SaveLoadManager.instance.ChangeDataHandler(slot);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
