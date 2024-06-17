/*
 * Avery
 */
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private string sceneToLoad;
    public static SceneLoader Instance;

    private void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    public void LoadScene(string scene)
    {
        LevelManager.Instance.LoadScene(scene);
    }

    public void LoadCutscene(string scene)
    {
        LevelManager.Instance.LoadCutscene(scene);
    }

    public void QueueScene(string scene)
    {
        sceneToLoad = scene;
    }

    public void LoadQueuedScene()
    {
        if (sceneToLoad != null)
            LoadScene(sceneToLoad);
    }

    public void ResetScene()
    {
        LevelManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }
}
