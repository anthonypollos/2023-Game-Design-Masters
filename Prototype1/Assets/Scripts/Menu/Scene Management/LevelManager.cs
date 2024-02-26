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

    private void Update()
    {
        progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, fillTarget, 1 * Time.deltaTime);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneName">Scene to Load</param>
    public async void LoadScene(string sceneName)
    {
        fillTarget = 0;
        progressBar.fillAmount = 0;

        loadScreen.SetActive(true);

        await Task.Delay(2);

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        while(!scene.isDone)
        {
            fillTarget = scene.progress + 0.1f;

            if (scene.progress >= 0.9f)
            {
                await Task.Delay(1);
                //fillTarget = 1.0f;
                scene.allowSceneActivation = true;
            }
        }

        await Task.Delay(1);

        loadScreen.SetActive(false);

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
