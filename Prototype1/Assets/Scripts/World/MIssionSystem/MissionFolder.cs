using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using FMODUnity;

public class MissionFolder : MonoBehaviour, ISaveable, IMissionContainer
{
    [SerializeField] bool isHub;
    
    [SerializeField] bool isTutorial;
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
    string previousMissionText = "";

    [SerializeField] string cutSceneOnFinishName;
    [SerializeField]
    private float fadeDelay = 1.0f;
    [Header("Needed only for Hub")]
    [SerializeField] string finalLevelName;
    public Vector3 checkPoint { get; private set; } = Vector3.zero;

    bool win = false;
    bool toggle = false;
    bool finalCutsceneWatched = false;


    Coroutine coroutineCheck;
    private StudioEventEmitter studioEventEmitter;
    Coroutine waitForBark;

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
                //Debug.Log(folder.name);
            }
            combatMissionActive = false;
            //currentDisplayedMission = 0;
            SetMission();
            DeveloperConsole.instance.SetMissionFolder(this);
        }
        studioEventEmitter = gameObject.GetComponent<StudioEventEmitter>();
        if (studioEventEmitter != null)
            studioEventEmitter.Stop();
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
        string previous = "";
        string text = "";
        for(int i = 0; i<=missionsCompleted; i++)
        {
            //If reached current mission
            if (i == missionsCompleted)
            {
                //if all missions finished
                if (missionsCompleted == missions.Count)
                {
                    text += "<s>" + previous + "</s>\nLeave the area";
                    return text;
                }
            }

            //hold new text
            string temp = missions[i].GetMissionText().Item1;
            MultiMissionBehavior multi;
            if(missions[i].TryGetComponent<MultiMissionBehavior>(out multi))
            {
                temp = multi.GetStandardMissionText();
            }
            //if new text is empty or same as its previous ignore
            //Debug.Log("Comparing " + previous + " with " + temp + " and got " + temp.CompareTo(previous));
            if (temp == string.Empty || temp.CompareTo(previous) == 0)
            {
                //Debug.Log("Empty or the same");
                continue;
            }
            //if new text is unique, strike out previous mission as completed and set new text as previous
            else
            { 
                if(previous!=string.Empty)
                    text += "<s>" + previous + "</s>\n";
                previous = temp;
            }
        }
        //combine previous to end
        return text + previous;
    }

    void NextMission()
    {
        if (!combatMissionActive)
        {
            currentDisplayedMission++;
            SetMission();
        }
    }
    
    public void AreaPlayVoiceClip(EventReference eventReference)
    {
        if(studioEventEmitter!=null)
        {
            studioEventEmitter.Stop();
            studioEventEmitter.ChangeEvent(eventReference);
            studioEventEmitter.Play();
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
        StartCoroutine(WaitTillNotFrozenCompletion(idx));
    }

    private IEnumerator WaitTillNotFrozenCompletion(int idx)
    {
        yield return new WaitUntil(() => Time.timeScale != 0);
        (EventReference, bool) temp = missions[idx].GetMissionCompleteVC();
        if (temp.Item2)
        {
            yield return new WaitUntil(() => !studioEventEmitter.IsPlaying());
            studioEventEmitter.Stop();
            studioEventEmitter.ChangeEvent(temp.Item1);
            studioEventEmitter.Play();
        }
        //SaveLoadManager.instance.SaveGame();
        if (missionsCompleted >= missions.Count)
            Victory();
        else if (currentDisplayedMission == idx)
            NextUnfinished();
        if(isHub)
        {
            if(missionsCompleted >= 1 && !finalCutsceneWatched)
            {
                Victory();
            }
        }
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
        StartCoroutine(WaitTillNotFrozenText());
    }

    private IEnumerator WaitTillNotFrozenText()
    {
        yield return new WaitUntil(() => Time.timeScale != 0);
        if (!combatMissionActive)
        {

            if(waitForBark!=null)
            {
                StopCoroutine(waitForBark);

            }
            MissionBehavior mission = missions[currentDisplayedMission];
            if(studioEventEmitter!=null)
                waitForBark = StartCoroutine(BarkTimer(mission.GetMin(), mission.GetMax()));
            string message = "";
            
            //bool temp = missionsStatuses[currentDisplayedMission];
            if (!missions[currentDisplayedMission].GetMissionText().Item2)
            {
                message = mission.GetMissionText().Item1;
            }
            else
            {
                message = mission.GetMissionText().Item1;
            }
            MultiMissionBehavior multi = mission.GetComponent<MultiMissionBehavior>();
            if(multi!=null)
            {
                string basicMessage = multi.GetStandardMissionText();
                if(basicMessage==string.Empty)
                {
                    message = previousMissionText + "\n" + message;
                }
                missionTextBox.text = message;
                if (basicMessage.CompareTo(previousMissionText) != 0 && basicMessage != string.Empty)
                {
                    string temp = "<s>" + previousMissionText + "</s>";
                    previousMissionText = basicMessage;
                }
               

            }
            //Only change text if mission's text is unique
            else if (message.CompareTo(previousMissionText) != 0 && message != string.Empty)
            {
                string temp = "<s>" + previousMissionText + "</s>";
                missionTextBox.text = message;
                previousMissionText = message;
                //Mission completion animation here
            }
            else
            {
                missionTextBox.text = previousMissionText;
            }
        }
        
    }

    [ContextMenu("Display GetText")]
    private void GetTextDebug()
    {
        Debug.Log(GetText());
    }

    private IEnumerator BarkTimer(float min, float max)
    {
        while (true)
        {
            float timer = Random.Range(min, max);
            yield return new WaitForSeconds(timer);
            (EventReference, bool) temp = missions[currentDisplayedMission].GetBark();
            if (temp.Item2)
            {
                yield return new WaitUntil(() => !studioEventEmitter.IsPlaying());
                studioEventEmitter.Stop();
                studioEventEmitter.ChangeEvent(temp.Item1);
                studioEventEmitter.Play();
            }
        }
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
            string temp = currentCombatMissionActive.GetMissionText().Item1;
            if(temp==string.Empty)
            {
                temp = previousMissionText;
            }
            string message = temp +
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
        {
            win = true;
            SaveLoadManager.instance.SaveGame();
        };
    }

    private void Victory()
    {
        
        if (!isHub && !win)
        {
            toggle = true;
        }
        win = true;
        SaveLoadManager.instance.SaveGame();
        Debug.Log("Victory!");
        if (!isHub)
        {
            if(coroutineCheck==null)
                coroutineCheck = StartCoroutine(FadeOut());
        }
        
        else
        {
            bool temp;
            SaveLoadManager.instance.GetCopy().levels.TryGetValue(finalLevelName, out temp);
            if(temp)
            {
                finalCutsceneWatched = true;
                if (coroutineCheck == null)
                    coroutineCheck = StartCoroutine(FadeOut());
            }
        }
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(fadeDelay);
        coroutineCheck = null;
        SceneLoader.Instance.LoadCutscene(cutSceneOnFinishName);
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
        if (checkPoint != Vector3.zero)
            DeveloperConsole.instance.playerRB.position = checkPoint;
        return true;
    }

    public void SetComplete()
    {
        for (int i = 0; i <= currentDisplayedMission; i++)
        {
            if (!missionsStatuses[i])
            {
                missions[i].OnComplete();
            }
        }
        if(checkPoint != Vector3.zero)
            DeveloperConsole.instance.playerRB.position = checkPoint;
    }

    public void SaveData(ref SavedValues savedValues)
    {
        if(savedValues.levels.ContainsKey(SceneManager.GetActiveScene().name))
        {
            savedValues.levels.Remove(SceneManager.GetActiveScene().name);
        }
        savedValues.hubReset = toggle;
        savedValues.levels.Add(SceneManager.GetActiveScene().name, win);

        savedValues.currentLevelMissionStatuses = missionsStatuses;
        savedValues.finalCutsceneWatched = finalCutsceneWatched;

    }

    

    public void LoadData(SavedValues savedValues)
    {
        finalCutsceneWatched = savedValues.finalCutsceneWatched;
        savedValues.levels.TryGetValue(SceneManager.GetActiveScene().name, out win);
        toggle = savedValues.hubReset;
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
            string lastString = "";
            missionsStatuses = savedValues.currentLevelMissionStatuses;
            for (int temp = 0; temp<missionsStatuses.Count; temp++)
            {
                //Debug.Log("Mission " + (temp+1).ToString() + " is completed: " + missionsStatuses[temp]);
                if (missionsStatuses[temp] && missions.Count > 0)
                {
                    missions[temp].SetFolder(this);
                    missions[temp].QuickSetToggles();
                    missions[temp].OnComplete();
                    string test = missions[temp].GetMissionText().Item1;
                    MultiMissionBehavior multi = missions[temp].GetComponent<MultiMissionBehavior>();
                    if (multi != null)
                    {
                        test = multi.GetStandardMissionText();
                    }
                    if (test != string.Empty && test.CompareTo(lastString) != 0)
                    {
                        lastString = test;
                    }
                    NextUnfinished();
                }
            }
                previousMissionText = lastString;
            //Debug.Log("Checkpoint location: " + checkPoint);
            
            if(checkPoint != Vector3.zero)
            {
                IsoPlayerController player = FindObjectOfType<IsoPlayerController>();
                player.GetComponent<Rigidbody>().position = checkPoint;
                //Debug.Log("Player Position: " + player.transform.position);
            }
            
        }
        if (isHub)
        {
            if (!toggle)
            {
                if (missionsCompleted == 0)
                {
                    missions[0].SetFolder(this);
                    missions[0].OnComplete();
                }
            }
            else
            {
                toggle = false;
            }
        }
    }
}
