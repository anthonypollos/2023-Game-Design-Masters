using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [SerializeField] private Animator anim;

    [SerializeField] private GameObject hoverTrigger;

    [SerializeField] private TextMeshProUGUI objectiveText;

    [SerializeField] private ObjectiveItem[] objectives;
    [SerializeField] private int index = 0;
    private ObjectiveItem currentObjective;

    [SerializeField] private float strikeSpeed;
    [SerializeField] private Image[] strikeThrus;

    private float strikeOneTarget, strikeTwoTarget;

    private int objShown = 0;

    //private GameController gc;

    private bool hoverActive;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        ToggleObjectiveHover(false);

        Invoke("DisplayFirstObjective", 0.25f);
    }

    /// <summary>
    /// 
    /// </summary>
    private void DisplayFirstObjective()
    {
        if (index==0)
            anim.SetTrigger("ObjFirst");
    }

    /// <summary>
    /// Completes current objective, triggers animation to advance to next objective
    /// </summary>
    /// /// <param name="objIndex">Objective index to complete</param>
    public void CompleteCurrentObjective(int objIndex)
    {
        if (objShown == 0)
        {
            anim.SetTrigger("ObjFirst");
            //index = objIndex;
        }

        else
        {
            anim.SetTrigger("ObjComplete");
            //index = objIndex + 1;
        }

        index = objIndex + 1;
    }

    /// <summary>
    /// Set objective to a certain index
    /// </summary>
    /// <param name="objIndex">Objective index to set</param>
    public void SetObjective(int objIndex)
    {
        if (objIndex < objectives.Length)
        {
            objectiveText.text = objectives[index].objectiveText;
            currentObjective = objectives[index];
        }
    }

    /// <summary>
    /// Called in objective complete animation, updates current objective/displayed objective text
    /// </summary>
    public void UpdateObjective()
    {
        SetObjective(index);

        objShown++;
    }

    /// <summary>
    /// Starts strike thru animaiton
    /// </summary>
    public void StartStrikeThru()
    {
        strikeThrus[0].fillAmount = 0;
        strikeThrus[1].fillAmount = 0;

        strikeOneTarget = objectives[index-1].strikeThruLengths[0];
        strikeTwoTarget = objectives[index-1].strikeThruLengths[1];

        StartCoroutine(StrikeThru());
    }

    public IEnumerator StrikeThru()
    {
        float strikeOneTarget = objectives[index-1].strikeThruLengths[0];
        float strikeTwoTarget = objectives[index-1].strikeThruLengths[1];

        while(strikeThrus[0].fillAmount < strikeOneTarget)
        {
            print("filling");

            strikeThrus[0].fillAmount = Mathf.Lerp(strikeThrus[0].fillAmount, (strikeOneTarget + 0.5f), strikeSpeed * Time.deltaTime);
            strikeThrus[1].fillAmount = Mathf.Lerp(strikeThrus[1].fillAmount, (strikeTwoTarget), strikeSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        strikeThrus[0].fillAmount = objectives[index-1].strikeThruLengths[0];
        strikeThrus[1].fillAmount = objectives[index-1].strikeThruLengths[1];

        yield return null;
    }
    
    public void ToggleHud(bool toggle)
    {
        if (toggle == true)
        {
            anim.SetBool("InCombat", false);
            ToggleObjectiveHover(true);
        }
        else if (toggle == false)
        {
            anim.SetBool("InCombat", true);
            ToggleObjectiveHover(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="toggle"></param>
    public void ToggleObjectiveHover(bool toggle)
    {
        if (toggle == hoverActive)
            return;

        // if toggling on, set InCombat anim bool to false, activate hover trigger
        if (toggle)
        {
            hoverActive = true;

            //anim.SetBool("InCombat", false);
            anim.SetBool("MouseOver", false);

            if (!hoverTrigger.activeInHierarchy)
                hoverTrigger.SetActive(true);
        }

        // if toggling off, set InCombat anim bool to true, deactivate hover trigger
        else if (!toggle)
        {
            hoverActive = false;

            //anim.SetBool("InCombat", true);
            anim.SetBool("MouseOver", false);

            if (hoverTrigger.activeInHierarchy)
                hoverTrigger.SetActive(false);
        }
    }

    /// <summary>
    /// Called in objective anims
    /// </summary>
    public void ActivateHover(int toggle)
    {
        if (toggle == 0)
            ToggleObjectiveHover(false);
        else if (toggle == 1)
            ToggleObjectiveHover(true);
    }
}



[System.Serializable]
public class ObjectiveItem
{
    public string objectiveText;

    public float[] strikeThruLengths = new float[2];
}