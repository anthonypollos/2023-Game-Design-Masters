using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMissionContainer
{
    public void MissionComplete(MissionBehavior mission);

    public void StartCombat(CombatMissionBehavior mission);

    public void CombatFinished();

    public void EnemyRemoved(GameObject enemy);

    public void UpdateMissionText();
}