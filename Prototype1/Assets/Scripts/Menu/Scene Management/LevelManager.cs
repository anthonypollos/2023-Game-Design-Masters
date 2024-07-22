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

    private float fillTarget;

    private string sceneToLoad;

    private bool isLoading = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
//            DontDestroyOnLoad(gameObject);
        }
//        else
//            Destroy(gameObject);
    }

    private void Update()
    {
        progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, fillTarget, 1 * Time.unscaledDeltaTime);
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
                fillTarget = scene.progress + 0.1f;

                //print(fillTarget);

                if (scene.progress >= 0.9f)
                {
                    await Task.Delay(1);
                    //fillTarget = 1.0f;
                    scene.allowSceneActivation = true;
                }
            }

            await Task.Delay(2);
            if (loadScreen != null)
                loadScreen.SetActive(false);

            Time.timeScale = 1;

            isLoading = false;

            //do {
            //    await Task.Delay(100);
            //    fillTarget = scene.progress;// + 0.1f;
            //} while (scene.progress < 0.9f);

            //await Task.Delay(1000);

            //scene.allowSceneActivation = true;

            //await Task.Delay(10);

            //loadScreen.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneName">Scene to Load</param>
    public async void LoadCutscene(string sceneName)
    {
        LoadScene(sceneName);
    }
}
