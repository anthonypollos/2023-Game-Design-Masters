/*
 * Avery
 */
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
            if(currentPage == 0)
            {
                backArrow.GetComponent<Image>().enabled = false;
                backArrow.GetComponent<Button>().enabled = false;
            }

            nextArrow.GetComponent<Image>().enabled = true;
            nextArrow.GetComponent<Button>().enabled = true;

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
        {
            page = 0;
        }
        else if (page == 0)
        {
            backArrow.GetComponent<Image>().enabled = false;
            backArrow.GetComponent<Button>().enabled = false;
            nextArrow.GetComponent<Image>().enabled = true;
            nextArrow.GetComponent<Button>().enabled = true;
        }
        else if (page >= textList.Length)
        {
            page = textList.Length - 1;
        }
        else if(page == textList.Length - 1)
        {
            backArrow.GetComponent<Image>().enabled = true;
            backArrow.GetComponent<Button>().enabled = true;
            nextArrow.GetComponent<Image>().enabled = false;
            nextArrow.GetComponent<Button>().enabled = false;
        }
        else
        {
            backArrow.GetComponent<Image>().enabled = true;
            backArrow.GetComponent<Button>().enabled = true;
            nextArrow.GetComponent<Image>().enabled = true;
            nextArrow.GetComponent<Button>().enabled = true;
        }

        currentPage = page;

        descriptionText.text = textList[page];

        pageDisplay.text = (currentPage + 1) + " / " + (textList.Length);
    }
}
