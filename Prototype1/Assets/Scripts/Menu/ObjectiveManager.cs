using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] private Animator anim;

    [SerializeField] private GameObject hoverTrigger;


    [SerializeField] private TextMeshProUGUI objectiveText;
    public string currentObjective, nextObjective;


    //private GameController gc;

    public bool inCombat;

    private bool hoverActive = true;

    // Start is called before the first frame update
    void Start()
    {
        //gc = FindObjectOfType<GameController>();

        ToggleObjectiveHover(true);

        // Mainly for testing, will likely get rid of this later
        Invoke("DisplayCurrentObjective", 0.25f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            ToggleObjectiveHover(true);

        else if (Input.GetKeyDown(KeyCode.D))
            ToggleObjectiveHover(false);

        else if (Input.GetKeyDown(KeyCode.W))
            anim.SetTrigger("ObjComplete");
    }

    public void SetCurrentObjective(string objective)
    {
        currentObjective = objective;
        objectiveText.text = currentObjective;
    }

    public void SetNextObjective(string objective)
    {
        nextObjective = objective;
    }

    public void DisplayCurrentObjective()
    {
        objectiveText.text = currentObjective;
    }

    /// <summary>
    /// 
    /// </summary>
    public void DisplayNextObjective()
    {
        objectiveText.text = nextObjective;
        currentObjective = nextObjective;
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

            anim.SetBool("InCombat", false);
            anim.SetBool("MouseOver", false);

            if (!hoverTrigger.activeInHierarchy)
                hoverTrigger.SetActive(true);
        }

        // if toggling off, set InCombat anim bool to true, deactivate hover trigger
        else if (!toggle)
        {
            hoverActive = false;

            anim.SetBool("InCombat", true);
            anim.SetBool("MouseOver", false);

            if (hoverTrigger.activeInHierarchy)
                hoverTrigger.SetActive(false);
        }
    }
}
