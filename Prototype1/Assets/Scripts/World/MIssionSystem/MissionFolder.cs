using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;
using TMPro;

public class MissionFolder : MonoBehaviour
{
    [SerializeField] List<MissionBehavior> missions;
    MenuControls controls;
    [SerializeField] Transform player;
    [SerializeField] GameObject wayFinder;
    [SerializeField] float wayFinderDistanceFromPlayer;
    bool[] missionsStatuses;
    [SerializeField] TextMeshProUGUI missionTextBox;
    bool combatMissionActive;
    CombatMissionBehavior currentCombatMissionActive;
    //[SerializeField] GameObject victoryMenu;
    int missionsCompleted;
    int currentDisplayedMission;

    private void OnEnable()
    {
        controls = new MenuControls();
        controls.Main.NextMission.Enable();
        controls.Main.PreviousMission.Enable();
        controls.Main.NextMission.performed += _ => NextMission();
        controls.Main.PreviousMission.performed += _ => PreviousMission();

    }
    private void OnDisable()
    {
        controls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        missionsCompleted = 0;
        missionsStatuses = new bool[missions.Count];
        FindObjectOfType<EnemyContainer>().SetMissionFolder(this);
        foreach(MissionBehavior folder in missions)
        {
            folder.SetFolder(this);
        }
        combatMissionActive = false;
        currentDisplayedMission = 0;
        SetMission();
    }

    private void Update()
    {
        if(!missionsStatuses[currentDisplayedMission])
        {
            Vector3 dir = (missions[currentDisplayedMission].transform.position - player.position).normalized;
            wayFinder.transform.position = player.position + dir * wayFinderDistanceFromPlayer;
            wayFinder.transform.forward = dir;
        }
    }

    void NextMission()
    {
        if (!combatMissionActive)
        {
            currentDisplayedMission++;
            SetMission();
        }
    }

    void NextUnfinished()
    {
        for (int i = currentDisplayedMission+1; i!=currentDisplayedMission; i++)
        {
            if (i > missions.Count)
                i = 0;
            else
            {
                if(!missionsStatuses[i])
                {
                    currentDisplayedMission = i;
                    SetMission();
                    break;
                }
            }
        }
        Debug.Log("Error, game not ending when all missions complete");
    }

    void PreviousMission()
    {
        if(!combatMissionActive)
        {
            currentDisplayedMission--;
            SetMission();
        }
    }

    public void MissionComplete(MissionBehavior mission)
    {
        missionsCompleted++;
        int idx = missions.IndexOf(mission);
        missionsStatuses[idx] = true;
        if (missionsCompleted >= missions.Count)
            Victory();
        else
            NextUnfinished();
    }

    void SetMission()
    {
       
        if(currentDisplayedMission < 0)
        {
            currentDisplayedMission = missions.Count - 1;
        }
        else if(currentDisplayedMission >= missions.Count)
        {
            currentDisplayedMission = 0;
        }
        if(missionsStatuses[currentDisplayedMission])
        {
            wayFinder.SetActive(false);
        }
        else
        {
            wayFinder.SetActive(true);
        }
        string temp = missionsStatuses[currentDisplayedMission] ? 
            "<color = green>Completed</color>" : "<color = orange>In Progress</color>";
        string message = "Current Status: " + temp + "\n" +
            missions[currentDisplayedMission].GetMissionText();
        missionTextBox.text = message;
    }

    public void EnemyRemoved(GameObject enemy)
    {
        if(combatMissionActive)
        {
            currentCombatMissionActive.RemoveEnemy(enemy.GetComponent<EnemyBrain>());
            CombatText();
        }
    }

    private void CombatText()
    {
        string message = "Current Status: <color = red>Active</color>\n" +
            currentCombatMissionActive.GetMissionText() +
            "\nEnemies Slain: " + currentCombatMissionActive.GetCount();
        missionTextBox.text = message;
    }

    public void StartCombat(CombatMissionBehavior mission)
    {
        currentDisplayedMission = missions.IndexOf(mission);
        combatMissionActive = true;
        currentCombatMissionActive = mission;
        CombatText();
    }

    public void CombatFinished()
    {
        missionsStatuses[missions.IndexOf(currentCombatMissionActive)] = true;
        currentCombatMissionActive = null;
        combatMissionActive = false;
        
    }

    private void Victory()
    {
        Debug.Log("Victory!");
    }

}
