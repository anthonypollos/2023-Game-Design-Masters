/*
 * Avery
 */
using UnityEngine;
using TMPro;

public class MultiPageDiary : MonoBehaviour
{
    public int maxPages = 1;
    public int currentPage = 0;

    [SerializeField] private TextMeshProUGUI pageDisplay;
    [SerializeField] private GameObject backArrow, nextArrow;

    [SerializeField] private DiaryInstance diaryInstance;

    private void OnEnable()
    {
        currentPage = 0;

        if (maxPages > 1)
        {
            backArrow.SetActive(true);
            nextArrow.SetActive(true);
            pageDisplay.gameObject.SetActive(true);
            pageDisplay.text = (currentPage + 1) + " / " + (maxPages);
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
        else if (page > maxPages)
            page = maxPages;

        currentPage = page;

        pageDisplay.text = (currentPage + 1) + " / " + (maxPages);

        diaryInstance.CheckEnable();
    }
}
