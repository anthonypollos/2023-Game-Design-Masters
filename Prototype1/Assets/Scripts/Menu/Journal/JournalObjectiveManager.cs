using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JournalObjectiveManager : MonoBehaviour
{
    public static JournalObjectiveManager Instance;

    [SerializeField] private JournalObjectiveItem[] objectives;

    public int index = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        for (int i = 1; i < objectives.Length; i++)
        {
            objectives[i].objectiveText.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.A))
            CompleteCurrentObjective();

        else if (Input.GetKeyDown(KeyCode.D))
            CompleteToObjective(5);
        */
    }

    /// <summary>
    /// Completes current objective; crosses off text and displays next objective
    /// </summary>
    public void CompleteCurrentObjective()
    {
        //CompleteObjective(index);
    }

    /// <summary>
    /// Completes objective at given index
    /// </summary>
    /// <param name="index">Index of objective to complete</param>
    public void CompleteObjective(int objIndex)
    {
        if(objIndex < objectives.Length)
        {
            JournalObjectiveItem currentObj = objectives[objIndex];

            currentObj.objectiveText.color = new Color(0.4811321f, 0.4366501f, 0.369927f);

            foreach (Transform child in currentObj.objectiveText.gameObject.transform)
            {
                GameObject strike = child.gameObject;
                strike.SetActive(true);
            }
            /*
            if(currentObj.nextObjective != null)
            {
                currentObj.nextObjective.SetActive(true);
            }
            */

            if(objIndex + 1 < objectives.Length)
            {
                objectives[objIndex + 1].objectiveText.gameObject.SetActive(true);
            }

            //index++;
        }
    }

    /// <summary>
    /// Completes all objectives up to given index, index included
    /// </summary>
    /// <param name="objIndex">Complete all objectives including this vaule</param>
    public void CompleteToObjective(int objIndex)
    {
        if(objIndex < objectives.Length)
        {
            for(int i = 0; i <= objIndex; i++)
            {
                CompleteObjective(i);
            }

            //index = objIndex;
        }
    }
}

[System.Serializable]
public class JournalObjectiveItem
{
    public TextMeshProUGUI objectiveText;
    //public GameObject[] strikes;

    //public GameObject nextObjective;
}
