/*
 * Avery
 */
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadScreen : MonoBehaviour
{
    [Header("Text Variables")]
    [SerializeField] private TextMeshProUGUI loadText;
    [SerializeField][TextArea] private string[] loadTexts;

    [Header("Image Variables")]
    [SerializeField] private Image loadImage;
    [SerializeField] private Sprite[] loadImages;

    // Start is called before the first frame update
    void Start()
    {
        Randomize();
    }

    private void Randomize()
    {
        int textMax = loadTexts.Length;
        int imageMax = loadTexts.Length;

        loadText.text = loadTexts[Random.Range(0, textMax)];
        loadImage.sprite = loadImages[Random.Range(0, imageMax)];
    }
}
