using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalButtonHelper : MonoBehaviour
{
    [SerializeField] Button[] buttons;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectButton(int index)
    {
        foreach(Button button in buttons)
        {
            button.interactable = true;
        }

        buttons[index].interactable = false;
    }
}
