/*
 * Avery
 */
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    private string sceneToLoad;

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
