/*
 * Avery
 */
using UnityEngine;

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

    public void QueueScene(string scene)
    {
        sceneToLoad = scene;
    }

    public void LoadQueuedScene()
    {
        if (sceneToLoad != null)
            LoadScene(sceneToLoad);
    }
}
