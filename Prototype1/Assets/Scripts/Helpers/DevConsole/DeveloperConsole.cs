using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

#region Commands
public abstract class ConsoleCommand: ScriptableObject
{ 
    [SerializeField] private string serializedCommandWord;
    [SerializeField] int argsMin = 0;
    [SerializeField] int argsMax = int.MaxValue;
    [SerializeField] protected string validCommandMessage;
    [SerializeField] private string invalidArgsMessage;
    [HideInInspector]
    public string commandWord => serializedCommandWord;

    public (bool, string) Process(string[] args)
    {

        if (args.Length >= argsMin && args.Length <= argsMax)
        {
            (bool, string) process = ActivateCommand(args);
            if (process.Item1)
            {
                return (true, process.Item2);
            }
            else
            {
                return (false, invalidArgsMessage);
            }
        }
        else
        {
            return (false, invalidArgsMessage);
        }
    }

    public abstract (bool, string) ActivateCommand(string[] args);
}


#endregion


public class DeveloperConsole : MonoBehaviour
{
    public static DeveloperConsole instance { get; private set; }
    [Header("Commands")]
    [SerializeField] List<ConsoleCommand> commands;

    [Header("UI")]
    [SerializeField] GameObject consoleUI;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI consoleTextField;
    private float previousTimeScale;

    MenuControls menuControls;

    //Required for commands
    public bool godMode = false;
    public GameObject player { get; private set; }
    public Rigidbody playerRB { get; private set; }
    public MissionFolder missionFolder { get; private set; }


    #region Variable Initializations
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    private void Start()
    {
        godMode = false;
        previousTimeScale = -1;
    }

    private void OnEnable()
    {
        if (menuControls == null)
            menuControls = new MenuControls();
        menuControls.Main.Console.Enable();
        menuControls.Main.Console.performed += _ => Toggle();
    }

    private void OnDisable()
    {
        menuControls.Main.Console.Disable();
    }

    public void SetPlayer(GameObject player, Rigidbody playerRB)
    {
        this.player = player;
        this.playerRB = playerRB;
    }

    public void SetMissionFolder(MissionFolder missionFolder)
    {
        this.missionFolder = missionFolder;
    }
    #endregion
    #region CommandHandling

    public void Toggle()
    {
        inputField.text = string.Empty;
        consoleTextField.text = string.Empty;
        consoleUI.SetActive(!consoleUI.activeInHierarchy);
        if(previousTimeScale >= 0)
        {
            Time.timeScale = previousTimeScale;
            previousTimeScale = -1;
        }
        else
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0;
            inputField.ActivateInputField();
        }
    }
    public void EnterCommand(string inputValue)
    {
        inputField.text = string.Empty;
        string[] inputSplit = inputValue.Split(' ');
        string commandInput = inputSplit[0];
        string[] args = inputSplit.Skip(1).ToArray();

        ProcessCommand(commandInput, args);
        inputField.ActivateInputField();
    }

    private void ProcessCommand(string commandInput, string[] args)
    {
        foreach(var command in commands)
        {
            if(!commandInput.Equals(command.commandWord, System.StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            (bool, string) process = command.Process(args);
            string message = process.Item2;
            if(process.Item1)
            {
                consoleTextField.text = message;
                return;
            }
            else
            {
                consoleTextField.text = "<color=red>Invalid Arguments\nExpected Format: " + message + "</color>";
                return;
            }
        }
        
        consoleTextField.text = "<color=red>" + "Unknown Command:\n" +commandInput+ " " + string.Join(" ",args) +  "</color>";
        return;
    }
    #endregion
}
