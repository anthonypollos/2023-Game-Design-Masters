using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContainer : MonoBehaviour
{
    private List<GameObject> aggroList;
    private List<GameObject> enemyList;
    private MissionFolder missionFolder;
    //private GameController gc;

    private void Awake()
    {
        aggroList = new List<GameObject>();
        enemyList = new List<GameObject>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //gc = FindObjectOfType<GameController>();
    }

    public void SetMissionFolder(MissionFolder temp)
    {
        missionFolder = temp;
    }

    public void AddEnemy(GameObject enemy)
    {
        if(!enemyList.Contains(enemy))
            enemyList.Add(enemy);
        //Debug.Log("Total Enemy Count: " + enemyList.Count);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (enemyList.Contains(enemy))
        {
            enemyList.Remove(enemy);
            missionFolder.EnemyRemoved(enemy);
        }
        
    }

    public void AddAggro(GameObject enemy)
    {
        if(!aggroList.Contains(enemy))
            aggroList.Add(enemy);
        GameController.instance.CombatState(true);
    }

    public void RemoveAggro(GameObject enemy)
    {
        if(aggroList.Contains(enemy))
            aggroList.Remove(enemy);
        if (aggroList.Count == 0)
            GameController.instance.CombatState(false);
    }
}
