using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMissionBehavior : MissionBehavior
{
    [SerializeField] List<EnemyBrain> enemies;
    int startingCount;
    [SerializeField] List<GameObject> arenaBarriers;
    bool completed;

    private void Awake()
    {
        startingCount = enemies.Count;
        completed = false;

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
        RemoveEnemy(null);
    }

    public string GetCount()
    {
        string message = startingCount - enemies.Count + "/" + startingCount;
        return message;
    }

    public void RemoveEnemy(EnemyBrain enemy)
    {
        if(enemies.Contains(enemy) && !completed)
        {
            enemies.Remove(enemy);
            if (enemies.Count <= 0)
                OnComplete();
        }
    }

    public override void OnComplete()
    {
        completed = true;
        if(enemies.Count>0)
        {
            foreach(EnemyBrain enemy in enemies)
            {
                enemy.gameObject.SetActive(false);
            }
        }
        enemies.Clear();
        foreach (GameObject barrier in arenaBarriers)
        {
            barrier.SetActive(false);
        }
        folder.CombatFinished();
        base.OnComplete();
    }
}
