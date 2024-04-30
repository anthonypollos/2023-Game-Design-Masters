/*
 * Avery
 */
using UnityEngine;
using TMPro;

public class MultiPageText : MonoBehaviour
{
    public string[] textList;
    public int currentPage = 0;

    public TextMeshProUGUI descriptionText, pageDisplay;

    public void SetPage(int value, string[] text)
    {
        currentPage = value;
        textList = text;

        descriptionText.text = textList[0];

        pageDisplay.text = (currentPage + 1) + " / " + (textList.Length);
    }

    public void TurnPage(int value)
    {
        int page = currentPage + value;

        if (page < 0)
            page = 0;
        else if (page >= textList.Length)
            page = textList.Length - 1;

        currentPage = page;

        descriptionText.text = textList[page];

        pageDisplay.text = (currentPage + 1) + " / " + (textList.Length);
    }
}
