using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public MenuBehavior mb;
    public string sceneToLoad;

    // Update is called once per frame
    void Start()
    {
        videoPlayer.loopPointReached += EndReached;
    }

    void EndReached(VideoPlayer videoPlayer)
    {
        mb.LoadScene(sceneToLoad);
    }
    

}
