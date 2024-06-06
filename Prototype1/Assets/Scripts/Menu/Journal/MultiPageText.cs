/*
 * Avery
 */
using UnityEngine;
using TMPro;

public class MultiPageText : MonoBehaviour
{
    public string[] textList;
    public int currentPage = 0;

    [SerializeField] private TextMeshProUGUI descriptionText, pageDisplay;
    [SerializeField] private GameObject backArrow, nextArrow;

    public void SetPage(int value, string[] text)
    {
        currentPage = value;
        textList = text;

        descriptionText.text = textList[0];

        if(textList.Length > 1)
        {
            backArrow.SetActive(true);
            nextArrow.SetActive(true);
            pageDisplay.gameObject.SetActive(true);
            pageDisplay.text = (currentPage + 1) + " / " + (textList.Length);
        }
        else
        {
            backArrow.SetActive(false);
            nextArrow.SetActive(false);
            pageDisplay.gameObject.SetActive(false);
        }
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
