using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : InteractableBehaviorTemplate//, ISaveable
{
    //[SerializeField] private GameObject levelSelectUI;
    [SerializeField] private UINavManager levelSelectMenuNav;
    // [SerializeField] private GameObject[] menus;

    public override bool Interact()
    {
        levelSelectMenuNav.OpenMainMenu();

        //ToggleLevelSelect();
        return false;
    }

    /*
    public void ToggleLevelSelect()
    {
        bool levelSelectActive = levelSelectUI.activeInHierarchy;

        // if level select active, disable
        if(levelSelectActive)
        {
            levelSelectUI.SetActive(false);
            Time.timeScale = 1;
            levelSelectActive = !levelSelectActive;
        }
        // if level select not active, enable
        else
        {
            levelSelectUI.SetActive(true);
            Time.timeScale = 0;
            levelSelectActive = !levelSelectActive;
        }

    }
    */

    //public void LoadData(SavedValues savedValues)
    //{
    //    bool temp = false;
    //    savedValues.levels.TryGetValue(worldName, out temp);
    //}


    //public void SaveData(ref SavedValues savedValues)
    //{
    //    bool temp = false;
    //    if (savedValues.levels.ContainsKey(worldName))
    //    {
    //        temp = savedValues.levels[worldName];
    //        savedValues.levels.Remove(worldName);
    //    }
    //    savedValues.levels.Add(worldName, temp);
    //}
}
