using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MultiMissionBehavior : MissionBehavior, IMissionContainer
{
    string combinedMissionText;
    [SerializeField] [Range(0, 100)] int objectiveTextModifier = 60;
    [SerializeField] List<MissionBehavior> objectives;
    List<bool> objectiveStatuses;
    CombatMissionBehavior currentCombatObjectiveActive;
    int objectivesCompleted;

    private void Awake()
    {
        objectivesCompleted = 0;
        currentCombatObjectiveActive = null;
        if (objectiveStatuses == null)
        {
            objectiveStatuses = new List<bool>();
            foreach (MissionBehavior objective in objectives)
            {
                objective.SetFolder(this);
                objectiveStatuses.Add(false);
            }
        }
    }

    public override (string, bool) GetMissionText()
    {
        UpdateObjectiveText();
        return (combinedMissionText, true);
    }

    public void EnemyRemoved(GameObject enemy)
    {
        foreach (MissionBehavior objective in objectives)
        {
            CombatMissionBehavior combatMissionBehavior = null;
            if (objective != null)
                combatMissionBehavior = objective.GetComponent<CombatMissionBehavior>();
            if (combatMissionBehavior != null)
            {
                combatMissionBehavior.RemoveEnemy(enemy.GetComponent<EnemyBrain>());

            }
        }
    }

    public void MissionComplete(MissionBehavior mission)
    {
        objectivesCompleted++;
        objectiveStatuses[objectives.IndexOf(mission)] = true;
        if (objectivesCompleted < objectives.Count)
            UpdateMissionText();
        else
            OnComplete();
    }

    public void StartCombat(CombatMissionBehavior mission)
    {
        currentCombatObjectiveActive = mission;
        UpdateMissionText();
    }

    public void CombatFinished()
    {
        currentCombatObjectiveActive = null;
    }

    public void UpdateObjectiveText()
    {
        if(objectivesCompleted >= objectives.Count) 
        {
            combinedMissionText = "<s>" + missionText + "</s>";

        }
        else
        {
            combinedMissionText =  missionText + "<size="+objectiveTextModifier+"%>";
            for(int i = 0; i< objectives.Count; i++)
            {
                if (objectiveStatuses[i])
                {
                    combinedMissionText+= ("        <s>" + objectives[i].GetMissionText().Item1 + "</s>");
                }
                else
                {
                    combinedMissionText += ("       "+
                        GetObjectiveText(objectives[i]));
                }
            }
            combinedMissionText += "</size>";
        }
    }

    public void UpdateMissionText()
    {
        folder.UpdateMissionText();
    }

    private string CombatText(CombatMissionBehavior combatObjective)
    {
        if (currentCombatObjectiveActive == combatObjective)
        {
            string message = combatObjective.GetMissionText().Item1 +
                "\nEnemies Slain: " + currentCombatObjectiveActive.GetCount();
            return message;
        }
        else
        {
            string message = combatObjective.GetMissionText().Item1;
            return message;
        }
    }

    private string GetObjectiveText(MissionBehavior objective)
    {
        CombatMissionBehavior combatMission = objective.GetComponent<CombatMissionBehavior>();
        if( combatMission != null )
        {
            return CombatText(combatMission);
        }
        else
        {
           return (objective.GetMissionText().Item1);
        }
    }

    public override void OnComplete()
    {
        if (objectiveStatuses == null)
        {
            objectiveStatuses = new List<bool>();
            for (int i = 0; i < objectives.Count; i++)
            {
                objectiveStatuses.Add(false);
            }
        }
        for (int temp = 0; temp < objectives.Count; temp++)
        {
            if (!objectiveStatuses[temp])
            {
                objectives[temp].SetFolder(this);
                objectives[temp].QuickSetToggles();
                objectives[temp].OnComplete();
            }
    
        }
        base.OnComplete();
    }
}
