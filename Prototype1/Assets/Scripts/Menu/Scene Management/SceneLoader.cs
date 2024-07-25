/*
 * Avery
 */
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private string sceneToLoad;
    public static SceneLoader Instance;

    bool loadCutscene = false;

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

    public void LoadSceneQuick(string scene)
    {
        LevelManager.Instance.LoadSceneQuick(scene);
    }

    public void QueueScene(string scene)
    {
        sceneToLoad = scene;
    }

    public void QueuedCutsceneCheck(bool scene)
    {
        loadCutscene = scene;
    }

    public void LoadQueuedScene()
    {
        if (sceneToLoad != null)
        {
            if (loadCutscene)
                LoadCutscene(sceneToLoad);
            else if (sceneToLoad.Equals("MainMenu_New") || sceneToLoad.Equals("HubScene"))
                LoadSceneQuick(sceneToLoad);
            else
                LoadScene(sceneToLoad);
        }
    }

    public void ResetScene()
    {
        LevelManager.Instance.LoadSceneQuick(SceneManager.GetActiveScene().name);
    }
}
