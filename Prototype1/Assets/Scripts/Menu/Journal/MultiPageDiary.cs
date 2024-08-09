/*
 * Avery
 */
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MultiPageDiary : MonoBehaviour
{
    public int maxPages = 1;
    public int currentPage = 1;

    [SerializeField] private TextMeshProUGUI pageDisplay;
    [SerializeField] private GameObject backArrow, nextArrow;

    [SerializeField] private DiaryPageInstance diaryInstance;

    private void OnEnable()
    {
        currentPage = 1;

        CheckMultiPage();
    }

    public void CheckMultiPage()
    {
        if (maxPages > 1)
        {
            backArrow.GetComponent<Image>().enabled = false;
            backArrow.GetComponent<Button>().enabled = false;
            nextArrow.GetComponent<Image>().enabled = true;
            nextArrow.GetComponent<Button>().enabled = true;

            backArrow.SetActive(true);
            nextArrow.SetActive(true);
            pageDisplay.gameObject.SetActive(true);
            pageDisplay.text = (currentPage) + " / " + (maxPages);
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

        if (page < 1)
        {
            page = 1;
        }
        else if (page == 1)
        {
            nextArrow.GetComponent<Image>().enabled = true;
            nextArrow.GetComponent<Button>().enabled = true;
            backArrow.GetComponent<Image>().enabled = false;
            backArrow.GetComponent<Button>().enabled = false;
        }
        else if (page > maxPages)
            page = maxPages;
        else if (page == maxPages)
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

        pageDisplay.text = (currentPage) + " / " + (maxPages);

        diaryInstance.CheckPage();
    }

    public void SetPage(int page)
    {
        currentPage = page;

        if (page < 1)
            currentPage = 1;
        else if(page == 1)
        {
            nextArrow.GetComponent<Image>().enabled = true;
            nextArrow.GetComponent<Button>().enabled = true;
            backArrow.GetComponent<Image>().enabled = false;
            backArrow.GetComponent<Button>().enabled = false;
        }
        else if (page > maxPages)
            currentPage = maxPages;
        else if(page == maxPages)
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

        pageDisplay.text = (currentPage) + " / " + (maxPages);
    }
}
