using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MissionFolder : MonoBehaviour, ISaveable, IMissionContainer
{
    [SerializeField] bool isHub;
    [SerializeField] List<MissionBehavior> missions;
    MenuControls controls;
    [SerializeField] Transform player;
    [SerializeField] GameObject wayFinder;
    [SerializeField] float wayFinderDistanceFromPlayer;
    List<bool> missionsStatuses;
    [SerializeField] TextMeshProUGUI missionTextBox;
    bool combatMissionActive;
    CombatMissionBehavior currentCombatMissionActive;
    //[SerializeField] GameObject victoryMenu;
    int missionsCompleted;
    int currentDisplayedMission = 0;
    public Vector3 checkPoint { get; private set; } = Vector3.zero;

    bool win = false;

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
        if (isHub)
            Win();
        if (missions.Count == 0)
        {
            win = true;
            SaveLoadManager.instance.SaveGame();
            this.enabled = false;
        }
        else
        {

            FindObjectOfType<EnemyContainer>().SetMissionFolder(this);
            foreach (MissionBehavior folder in missions)
            {
                folder.SetFolder(this);
                Debug.Log(folder.name);
            }
            combatMissionActive = false;
            //currentDisplayedMission = 0;
            SetMission();
            DeveloperConsole.instance.SetMissionFolder(this);
        }
    }

    private void Update()
    {
        if(!missionsStatuses[currentDisplayedMission])
        {
            Vector3 dir = (missions[currentDisplayedMission].transform.position - player.position).normalized;
            dir.y = 0;
            //wayFinder.transform.position = player.position + dir * wayFinderDistanceFromPlayer + Vector3.up * 2;
            //wayFinder.transform.forward = dir;
        }
    }

    public string GetText()
    {
        string text = "<s>";
        for(int i = 0; i<=missionsCompleted; i++)
        {
            if (i == missionsCompleted)
            {
                text += "</s>";
                if (missionsCompleted == missions.Count)
                {
                    text += "Leave the area";
                }
            }
            else
            {
                text += missions[i].GetMissionText().Item1 + "\n";
            }
        }
        return text;
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
        for (int i = 0; i<missions.Count; i++)
        {
            int temp = i + currentDisplayedMission;
            temp = temp%missions.Count;
            if (i > missions.Count)
                i = 0;
            else
            {
                if (temp >= 0 && temp < missions.Count)
                {
                    if (!missionsStatuses[temp])
                    {
                        currentDisplayedMission = temp;
                        SetMission();
                        return;
                    }
                }
                else
                {
                    Debug.Log("Values goes past, possible serialze error");
                }
            }
        }
        //Debug.Log("Error, game not ending when all missions complete");
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
        checkPoint = mission.checkPointLocation;
        //SaveLoadManager.instance.SaveGame();
        if (missionsCompleted >= missions.Count)
            Victory();
        else if (currentDisplayedMission == idx)
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
        UpdateMissionText();
        /*
        if (missionsStatuses[currentDisplayedMission])
        {
            wayFinder.SetActive(false);
        }
        else
        {
            wayFinder.SetActive(true);
        }
        */
    }

    public void UpdateMissionText()
    {
        string message = "";
        bool temp = missionsStatuses[currentDisplayedMission];
        if (!missions[currentDisplayedMission].GetMissionText().Item2)
        {
            message = missions[currentDisplayedMission].GetMissionText().Item1;
        }
        else
        {
            message = missions[currentDisplayedMission].GetMissionText().Item1;
        }
        if (temp)
            message = "<s>" + message + "</s>";
        missionTextBox.text = message;
    }

    public void EnemyRemoved(GameObject enemy)
    {
        foreach (MissionBehavior mission in missions)
        {
            CombatMissionBehavior combatMissionBehavior = null;
            if(mission != null) 
                 combatMissionBehavior = mission.GetComponent<CombatMissionBehavior>();
            if (combatMissionBehavior != null)
            {
                combatMissionBehavior.RemoveEnemy(enemy.GetComponent<EnemyBrain>());

            }
            MultiMissionBehavior multiMissionBehavior = null;
            if(mission != null)
                multiMissionBehavior = mission.GetComponent<MultiMissionBehavior>();
            if(multiMissionBehavior != null)
            {
                multiMissionBehavior.EnemyRemoved(enemy);
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
            string message = currentCombatMissionActive.GetMissionText().Item1 +
                "\nEnemies Slain: " + currentCombatMissionActive.GetCount();
            missionTextBox.text = message;
        }
    }

    public void StartCombat(CombatMissionBehavior mission)
    {
        //wayFinder.SetActive(false);
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

    public void Win()
    {
        if (!win)
            Victory();
    }

    private void Victory()
    {
        win = true;
        SaveLoadManager.instance.SaveGame();
        //Debug.Log("Victory!");
    }

    public bool SetComplete(int mission)
    {
        if(mission>missions.Count)
        {
            return false;
        }
        else
        {
            for (int i = 0; i<mission; i++)
            {
                if(!missionsStatuses[i])
                {
                    missions[i].OnComplete();
                }
            }
        }
        return true;
    }

    public void SaveData(ref SavedValues savedValues)
    {
        if(savedValues.levels.ContainsKey(SceneManager.GetActiveScene().name))
        {
            savedValues.levels.Remove(SceneManager.GetActiveScene().name);
        }
        savedValues.levels.Add(SceneManager.GetActiveScene().name, win);

        savedValues.currentLevelMissionStatuses = missionsStatuses;

    }

    

    public void LoadData(SavedValues savedValues)
    {
        savedValues.levels.TryGetValue(SceneManager.GetActiveScene().name, out win);
        missionsCompleted = 0;
        if(savedValues.currentLevelMissionStatuses.Count == 0)
        {
            Debug.Log("No missions found");
            bool[] temp = new bool[missions.Count];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = false;
            missionsStatuses = new List<bool>(temp);
        }
        else
        {
            checkPoint = Vector3.zero;
            missionsStatuses = savedValues.currentLevelMissionStatuses;
            for (int temp = 0; temp<missionsStatuses.Count; temp++)
            {
                //Debug.Log("Mission " + (temp+1).ToString() + " is completed: " + missionsStatuses[temp]);
                if (missionsStatuses[temp] && missions.Count>0)
                {
                    missions[temp].SetFolder(this);
                    missions[temp].QuickSetToggles();
                    missions[temp].OnComplete();
                }
            }
            Debug.Log("Checkpoint location: " + checkPoint);
            
            if(checkPoint != Vector3.zero)
            {
                IsoPlayerController player = FindObjectOfType<IsoPlayerController>();
                player.GetComponent<Rigidbody>().position = checkPoint;
                Debug.Log("Player Position: " + player.transform.position);
            }
            
        }
    }
}
