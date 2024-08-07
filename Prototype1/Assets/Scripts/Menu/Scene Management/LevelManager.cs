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

    [Header("CutsceneTransition")]
    [SerializeField] private GameObject cutsceneTransition;
    [SerializeField] private Animator cutsceneTransAnim;

    private float fillTarget;

    private string sceneToLoad;

    private bool isLoading = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
       // else
            //Destroy(gameObject);
    }

    private void Update()
    {
        if(progressBar != null)
            progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, fillTarget, 0.3f * Time.unscaledDeltaTime);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneName">Scene to Load</param>
    public async void LoadScene(string sceneName)
    {
        if(!isLoading)
        {
            isLoading = true;

            Time.timeScale = 0;

            fillTarget = 0;
            progressBar.fillAmount = 0;

            loadScreen.SetActive(true);

            await Task.Delay(2);

            var scene = SceneManager.LoadSceneAsync(sceneName);
            scene.allowSceneActivation = false;

            while (!scene.isDone)
            {
                fillTarget = scene.progress + 0.3f;

                if (scene.progress >= 0.9f)
                {
                    await Task.Delay(4200);

                    fillTarget = 1.0f;
                    progressBar.fillAmount = 1.0f;

                    break;
                }
            }

            scene.allowSceneActivation = true;
            isLoading = false;
            Time.timeScale = 1;

            await Task.Delay(2);

            if (loadScreen != null)
                loadScreen.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneName">Scene to Load</param>
    public async void LoadSceneQuick(string sceneName)
    {
        LoadScene(sceneName);
        /*
        if (!isLoading)
        {
            isLoading = true;

            Time.timeScale = 0;

            fillTarget = 0;
            progressBar.fillAmount = 0;

            loadScreen.SetActive(true);

            await Task.Delay(2);

            var scene = SceneManager.LoadSceneAsync(sceneName);
            scene.allowSceneActivation = false;

            while (!scene.isDone)
            {
                fillTarget = scene.progress + 0.05f;

                if (scene.progress >= 0.9f)
                {
                    await Task.Delay(1500);

                    fillTarget = 1.0f;
                    progressBar.fillAmount = 1.0f;

                    break;
                }
            }

            scene.allowSceneActivation = true;
            isLoading = false;
            Time.timeScale = 1;

            await Task.Delay(2);

            if (loadScreen != null)
                loadScreen.SetActive(false);
        }
        */
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneName">Scene to Load</param>
    public async void LoadCutscene(string sceneName)
    {
        if (!isLoading)
        {
            isLoading = true;

            Time.timeScale = 0;

            cutsceneTransition.SetActive(true);

            await Task.Delay(500);

            var scene = SceneManager.LoadSceneAsync(sceneName);
            scene.allowSceneActivation = false;

            while (!scene.isDone)
            {
                if (scene.progress >= 0.9f)
                {
                    await Task.Delay(500);

                    scene.allowSceneActivation = true;
                    Time.timeScale = 1;
                    isLoading = false;
                }
            }

        }
    }
}
