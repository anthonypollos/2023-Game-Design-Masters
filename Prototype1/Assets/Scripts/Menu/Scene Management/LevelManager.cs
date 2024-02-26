/*
 * Avery
 */
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Load Screen UI")]
    [SerializeField] private GameObject loadScreen;
    [SerializeField] private Image progressBar;

    private string sceneToLoad;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneName">Scene to Load</param>
    public async void LoadScene(string sceneName)
    {
        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;
        loadScreen.SetActive(true);

        do {
            await Task.Delay(100);        // delete this
            progressBar.fillAmount = scene.progress + 0.1f;
        } while (scene.progress < 0.9f);

        scene.allowSceneActivation = true;
        loadScreen.SetActive(false);
    }
}
