using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HubObjectivePage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headerText, levelSummary;

    [SerializeField] private string[] headerTexts;
    [SerializeField][TextArea(3,6)] private string[] levelSummaries;

    // Start is called before the first frame update
    void OnEnable()
    {
        CheckLevels();
    }

    private void CheckLevels()
    {
        SavedValues temp = SaveLoadManager.instance.GetCopy();
        int index = 0;

        bool beatRail = temp.levels.TryGetValue("C_ArtPass_railyard_v2", out beatRail);

        bool beatTown = temp.levels.TryGetValue("Town_Connor_Art_Pass", out beatTown);

        bool beatTut = temp.levels.TryGetValue("Tutorial_new", out beatTut);

        if (beatRail)
            index = 2;
        else if (beatTown)
            index = 1;
        else if (beatTut)
            index = 0;

        headerText.text = headerTexts[index];
        levelSummary.text = levelSummaries[index];
    }
}
