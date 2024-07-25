/*
 * Avery
 */
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadScreen : MonoBehaviour
{
    [SerializeField] bool isCutscene = false;
    [SerializeField] bool isCutsceneScene = false;

    private MainControls mainCont;
    private MenuControls menuCont;

    [Header("Text Variables")]
    [SerializeField] private TextMeshProUGUI tooltipText;
    private string[] tooltips = new string[11];

    [Header("Image Variables")]
    [SerializeField] private Image loadImage;
    [SerializeField] private Sprite[] loadImages;

    // Start is called before the first frame update
    void Start()
    {
        if(!isCutscene)
        {
            mainCont = ControlsContainer.instance.mainControls;
            menuCont = new MenuControls();

            SetTooltips();

            Randomize();
        }

        if (isCutsceneScene)
            GetComponent<Animator>().SetBool("Outro", true);
    }

    private void Randomize()
    {
        int textMax = tooltips.Length;
        int imageMax = loadImages.Length;

        tooltipText.text = tooltips[Random.Range(0, textMax)];
        loadImage.sprite = loadImages[Random.Range(0, imageMax)];
    }

    private void SetTooltips()
    {
        string dashSprite = mainCont.Main.Dash.bindings[0].ToDisplayString().ToUpper().TranslateToSprite();
        string journalSprite = menuCont.Main.Journal.bindings[0].ToDisplayString().ToUpper().TranslateToSprite();

        string[] texts = new string[]
        {
        "Don't forget to stop, drop, and roll. Pressing<size=26>" + dashSprite + "</size>while on fire will put you out.",
        "Throwing objects at enemies packs more of a punch than the other way around.",
        "If you're lost or stumped, check the scrap of paper in the top left corner or press<size=26>" + journalSprite + "</size>to open the journal.",
        "Running over healing vials works just fine, but you can also grab them with your tendril and pull them to you.",
        "Fire spreads between flammable things on contact. That includes wooden objects, other enemies, and you.",
        "Some objects are heavier than others, but they'll become easier to throw the longer you drag them.",
        "Broken glass hurts anyone that moves over it, be they friend or foe.",
        "If you're not sure where to go, following the trail of destruction is usually a safe bet.",
        "Enemies are quick to break out of your tendril's grasp. If you grab one, think fast.",
        "Whatever you do, DO NOT JUMP IN THE PIT.",
        "When a horde of monsters is chasing you, don't panic. Look around for something that will hit a lot of enemies at once."
        };

        tooltips = texts;
    }

    public void DisableLoadScreen()
    {
        gameObject.SetActive(false);
    }
}
