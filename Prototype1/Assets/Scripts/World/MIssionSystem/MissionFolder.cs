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
        if (missions.Count == 0)
            this.enabled = false;
        else
        {
            missionsCompleted = 0;
            missionsStatuses = new bool[missions.Count];
            FindObjectOfType<EnemyContainer>().SetMissionFolder(this);
            foreach (MissionBehavior folder in missions)
            {
                folder.SetFolder(this);
            }
            combatMissionActive = false;
            currentDisplayedMission = 0;
            SetMission();
        }
    }

    private void Update()
    {
        if(!missionsStatuses[currentDisplayedMission])
        {
            Vector3 dir = (missions[currentDisplayedMission].transform.position - player.position).normalized;
            dir.y = 0;
            wayFinder.transform.position = player.position + dir * wayFinderDistanceFromPlayer + Vector3.up * 2;
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
            if (i >= missions.Count)
                i = 0;
            else
            {
                if(!missionsStatuses[i])
                {
                    currentDisplayedMission = i;
                    SetMission();
                    return;
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
        SetMission();
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
            "<color=green>Completed</color>" : "<color=orange>In Progress</color>";
        string message = "Current Status: " + temp + "\n" +
            missions[currentDisplayedMission].GetMissionText();
        missionTextBox.text = message;
        if (!missionsStatuses[currentDisplayedMission])
        {
            wayFinder.SetActive(true);
        }
    }

    public void EnemyRemoved(GameObject enemy)
    {
        foreach (MissionBehavior mission in missions)
        {
            CombatMissionBehavior combatMissionBehavior = mission.GetComponent<CombatMissionBehavior>();
            if (combatMissionBehavior != null)
            {
                combatMissionBehavior.RemoveEnemy(enemy.GetComponent<EnemyBrain>());

            }
        }
        //currentCombatMissionActive.RemoveEnemy(enemy.GetComponent<EnemyBrain>());
        if (combatMissionActive)
        {
            CombatText();
        }
    }

    private void CombatText()
    {
        if (combatMissionActive)
        {
            string message = "Current Status: <color=red>Active</color>\n" +
                currentCombatMissionActive.GetMissionText() +
                "\nEnemies Slain: " + currentCombatMissionActive.GetCount();
            missionTextBox.text = message;
        }
    }

    public void StartCombat(CombatMissionBehavior mission)
    {
        wayFinder.SetActive(false);
        currentDisplayedMission = missions.IndexOf(mission);
        combatMissionActive = true;
        currentCombatMissionActive = mission;
        CombatText();
    }

    public void CombatFinished()
    {
        if (currentCombatMissionActive)
        {
            missionsStatuses[missions.IndexOf(currentCombatMissionActive)] = true;
            currentCombatMissionActive = null;
            combatMissionActive = false;
        }
        
        
    }

    private void Victory()
    {
        Debug.Log("Victory!");
    }

}
