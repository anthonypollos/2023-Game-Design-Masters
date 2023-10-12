using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMissionBehavior : MissionBehavior
{
    [SerializeField] List<EnemyBrain> enemies;
    int startingCount;
    [SerializeField] List<GameObject> arenaBarriers;

    private void Awake()
    {
        startingCount = enemies.Count;

    }
    protected override void OnTriggered()
    {
        StartCombat();
    }

    void StartCombat()
    {
        foreach (EnemyBrain enemy in enemies)
        {
            enemy.Aggro();
        }
        foreach (GameObject barrier in arenaBarriers)
        {
            barrier.SetActive(true);
        }
        folder.StartCombat(this);
    }

    public string GetCount()
    {
        string message = startingCount - enemies.Count + "/" + startingCount;
        return message;
    }

    public void RemoveEnemy(EnemyBrain enemy)
    {
        if(enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
        if (enemies.Count <= 0)
            OnComplete();
    }

    protected override void OnComplete()
    {
        foreach (GameObject barrier in arenaBarriers)
        {
            barrier.SetActive(false);
        }
        folder.CombatFinished();
        base.OnComplete();
    }
}
